using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public enum EnemyStates
{
    Wander,
    Seek,
    Destroy
}

public class PathfindingNode
{
    public Vector2 coordinates;

    // Values
    public float
        G = -1,
        H = 0,
        F = -1;

    public PathfindingNode(Vector2 coordinates)
    {
        this.coordinates = coordinates;
    }

}

public class EnemyStateMachineScript : MonoBehaviour
{
    [Header("State Machine Variable")]
    [SerializeField] private EnemyStates _currentState;
    [SerializeField] private float _pathfindingAngleIncrement;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _body;
    [SerializeField] private CircleCollider2D _rangeCollider;
    [SerializeField] private CircleCollider2D _bodyCollision;

    [Header("Wander State Variable")]
    [SerializeField] private float _wanderTime;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _acceleration;

    [Header("Seek State Variable")]
    [SerializeField] private List<Transform> _targets = new List<Transform>();
    [SerializeField] private float _entityDetectionRange;
    [SerializeField] private float _approachTargetRange;

    [Header("Destroy State Variable")]
    [SerializeField] private GunState _gunState;
    [SerializeField] private GunData _heldGun;
    [SerializeField] private List<GunData> _gunData;
    [SerializeField] private Transform _bulletSpawnPoint;

    [SerializeField] private float _currentWanderTime;
    private float _currentFireTimer;
    private float _currentReloadTimer;

    private Vector2 _targetLocation;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rangeCollider = GetComponent<CircleCollider2D>();
        _rangeCollider.radius = _entityDetectionRange;

        _currentWanderTime = _wanderTime;
        
    }

    private void Start()
    {
        EnterWanderState();

        InitializeGun();
    }

    private void FixedUpdate()
    {
        switch(_currentState)
        {
            case EnemyStates.Wander:

                WanderState();
                break;

            case EnemyStates.Seek:

                SeekState();
                break;

            case EnemyStates.Destroy:

                DestroyState();
                break;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EntityStatsScript>() != null)
        {
            _targets.Add(collision.transform);

            EnterSeekState();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<EntityStatsScript>() != null)
        {
            _targets.Remove(collision.transform);

            if (_targets.Count <= 0)
                EnterWanderState();
        }
    }

    private void InitializeGun()
    {
        _heldGun = _gunData[Random.Range(0, _gunData.Count)];
        _heldGun.currentLoadedBullets = _heldGun.MagSize;
    }

    private void WanderState()
    {
        Debug.Log("Updating Wander");

        if (_currentWanderTime > 0)
        {
            
            TraversePath();
            _currentWanderTime -= Time.deltaTime;
        }
        else
        {
            EnterWanderState();
            _currentWanderTime = _wanderTime;
        }

    }

    private void SeekState()
    {
        _targetLocation = _targets[0].position;
        TraversePath();

        if (Vector2.Distance(_targets[0].position, transform.position) <= _approachTargetRange)
            EnterDestroyState();
    }

    private void DestroyState()
    {
        // Aim at enemy
        RotateEntity(_targets[0].position);

        // Fire or reload
        switch (_gunState)
        {
            case GunState.Firing:

                FiringGunSubState();
                break;

            default:
                ReloadingGunSubState();
                break;
        }

        if (Vector2.Distance(_targets[0].position, transform.position) > _approachTargetRange)
        {
            _targets.Sort((targetA, targetB) => (Vector2.Distance(targetA.position, transform.position).CompareTo(Vector2.Distance(targetB.position, transform.position))) );
            if (Vector2.Distance(_targets[0].position, transform.position) > _approachTargetRange)
                EnterSeekState();
        }
    }

    private void FiringGunSubState()
    {
        if (_currentFireTimer == _heldGun.FireRate)
        {

            if (_heldGun == null)
                return;

            ShootBullet();
            _currentFireTimer -= Time.deltaTime;

            if (_heldGun.currentLoadedBullets <= 0)
            {
                _gunState = GunState.Reloading;
                return;
            }

        }
        else if (_currentFireTimer > 0)
            _currentFireTimer -= Time.deltaTime;
        else
        {
            _currentFireTimer = _heldGun.FireRate;
            if (_heldGun.Mode == GunMode.Semi_Automatic)
                _gunState = GunState.Idle;
        }
    }

    private void ReloadingGunSubState()
    {
        if (_currentReloadTimer > 0)
            _currentReloadTimer -= Time.deltaTime;
        else
        {
            _heldGun.currentLoadedBullets = _heldGun.MagSize;
            _gunState = GunState.Firing;
        }
    }


    private void EnterWanderState()
    {
        _targetLocation = FindRandomDestination();

        _currentState = EnemyStates.Wander;

        Debug.Log("Entered Wander");
    }

    private void EnterSeekState()
    {
        _targetLocation = _targets[0].position;
        _currentState = EnemyStates.Seek;
    }

    private void EnterDestroyState()
    {
        _currentState = EnemyStates.Destroy;
    }

    private Vector2 FindRandomDestination()
    {

        // For wander state only
        _currentWanderTime = _wanderTime;

        // Pick a random point inside the target range
        Vector2 randomPoint = Vector2.zero;

        do
        {
            Vector2 randomDirection = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.right;
            randomPoint = randomDirection * Random.Range(0, _rangeCollider.radius);

        } while (Physics2D.OverlapCircle(randomPoint, _bodyCollision.radius, LayerMask.GetMask("Obstacles")) != null);

        Debug.Log("Random dest: " + randomPoint);

        return randomPoint;
    }

    private void TraversePath()
    {
        Vector2 direction = _targetLocation - new Vector2(transform.position.x, transform.position.y);
        direction.Normalize();

        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, direction * _movementSpeed, _acceleration);

        RotateEntity(direction);

        // If there is an obstacle in the way
        if (Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(_targetLocation.y, _targetLocation.x) * Mathf.Rad2Deg)) <  0.5f)
            if (Physics2D.Raycast(transform.position, _body.right, _bodyCollision.radius * 2, LayerMask.GetMask("Obstacles")))
                _targetLocation = FindRandomDestination();

        // If the enemy is near the location
        if (Vector2.Distance(_targetLocation, new Vector2(transform.position.x, transform.position.y)) < 1)
            _targetLocation = FindRandomDestination();

    }

    private void RotateEntity(Vector2 target)
    {
        _body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg), _acceleration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + _body.right * _movementSpeed);

        Gizmos.DrawWireSphere(_targetLocation, _bodyCollision.radius);
    }

    private void ShootBullet()
    {
        _heldGun.currentLoadedBullets--;

        if (_heldGun.Type == GunType.Shotgun)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = Random.Range(-_heldGun.ShotgunArcAngle, _heldGun.ShotgunArcAngle);
                Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
                BulletScript newBullet = Instantiate(_heldGun.BulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
                newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(rotatedDirection), _heldGun.Damage);
            }
        }
        else
        {
            BulletScript newBullet = Instantiate(_heldGun.BulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
            newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(Vector2.up), _heldGun.Damage);
        }


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyStates
{
    Wander,
    Seek,
    Destroy
}

public class EnemyFSMScript : MonoBehaviour
{
    [Header("State Machine Variable")]
    [SerializeField] private EnemyStates _currentState;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _body;
    [SerializeField] private CircleCollider2D _rangeCollider;
    [SerializeField] private float _pathfindingAngleIncrement;
    [SerializeField] private float _bodyCollisionRadius;
    [SerializeField] private bool _isBoss = false;

    [Header("Wander State Variable")]
    [SerializeField] private float _wanderTime;
    [SerializeField] private float _currentWanderTime;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _acceleration;

    [Header("Seek State Variable")]
    [SerializeField] private List<Transform> _entitiesDetected = new List<Transform>();
    [SerializeField] private float _entityDetectionRange;
    [SerializeField] private float _attackTargetRange;

    [Header("Destroy State Variable")]
    [SerializeField] private GunState _gunState;
    [SerializeField] private GunData _heldGun;
    [SerializeField] private SpriteRenderer _heldGunSprite;
    [SerializeField] private Transform _bulletSpawnPoint;

    [Header("Misc")]
    [SerializeField] private ParticleSystem _muzzleParticles;

    private float _currentFireTimer;
    private float _currentReloadTimer;
    private Vector2 _targetLocation;

    private void Awake()
    {
        _rangeCollider = GetComponent<CircleCollider2D>();
        _rangeCollider.radius = _entityDetectionRange;

        _currentWanderTime = _wanderTime;
        
    }

    private void Start()
    {
        EnterWanderState();
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
        if (collision.GetComponent<EntityStatsScript>() != null && collision.gameObject != _rigidbody.gameObject) // Stop it detecting itself
        {
            // Add it to the end of the list
            _entitiesDetected.Insert(_entitiesDetected.Count, collision.transform);

            EnterSeekState();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<EntityStatsScript>() != null && collision.gameObject != _rigidbody.gameObject) // Stop it detecting itself
        {
            _entitiesDetected.Remove(collision.transform);

            if (_entitiesDetected.Count <= 0)
                EnterWanderState();
        }
    }

    public void InitializeGun(GunData newGun)
    {
        // Reference the given gun and load its ammo
        _heldGun = newGun;
        _heldGun.currentLoadedBullets = _heldGun.MagSize;

        // Reset the fire rate
        _currentFireTimer = _heldGun.FireRate;

        // Make the boss reload according to its fire rate (rocket launcher), as its reload time is for the player
        if (_isBoss)
            _currentReloadTimer = _heldGun.FireRate;
        else
            _currentReloadTimer = _heldGun.ReloadSpeed;

        // Initialize the sprite
        _heldGunSprite.sprite = _heldGun.GunHoldSprite;
    }

    private void EnterWanderState()
    {
        _targetLocation = FindRandomDestination();

        _currentState = EnemyStates.Wander;
    }

    private void EnterSeekState()
    {
        _targetLocation = _entitiesDetected[0].position;
        _currentState = EnemyStates.Seek;
    }

    private void EnterDestroyState()
    {
        _rigidbody.velocity = Vector3.zero;
        _currentState = EnemyStates.Destroy;
    }

    private void WanderState()
    {

        if (_currentWanderTime > 0)
        {
            TraversePathTo(_targetLocation);

            // If the target is near the location, find another destination
            if (Vector2.Distance(_targetLocation, new Vector2(transform.position.x, transform.position.y)) < 1)
                _targetLocation = FindRandomDestination();

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
        TraversePathTo(_entitiesDetected[0].position);

        if (_entitiesDetected.Count <= 0)
        {
            EnterWanderState();
            return;
        }

        if (Vector2.Distance(_entitiesDetected[0].position, transform.position) <= _attackTargetRange)
            EnterDestroyState();
    }

    private void DestroyState()
    {
        // Lerp velocity to 0
        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, 0.5f);
        if (Mathf.Abs(Vector3.zero.magnitude - _rigidbody.velocity.magnitude) < 0.2)
            _rigidbody.velocity = Vector3.zero;

        // Aim at enemy
        Vector2 direction = (_entitiesDetected[0].position - transform.position).normalized;
        RotateEntity(direction);

        // Detect if an obstacle is in front of an enemy, and change directions if so
        if (Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)) < 5f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _bodyCollisionRadius * 2, LayerMask.GetMask("Obstacles"));
            if (hit.collider != null)
            {
                Debug.Log("Obstacle Encountered!");

                // Remove the entity detected if it is obscured
                _entitiesDetected.RemoveAt(0);
                if (_entitiesDetected.Count <= 0)
                {
                    EnterWanderState();
                    return;
                }
                else
                {
                    EnterSeekState();
                    return;
                }
            }

        }

        // Fire or reload states of the gun
        switch (_gunState)
        {
            case GunState.Firing:

                FiringGunSubState();
                break;

            default:
                ReloadingGunSubState();
                break;
        }

        if (_entitiesDetected.Count <= 0)
        {
            EnterWanderState();
            return;
        }

        // If the main target has exited the attack range
        if (Vector2.Distance(_entitiesDetected[0].position, transform.position) > _attackTargetRange)
        {
            // Sort all entities detected by order of their distance to the entity.
            _entitiesDetected.Sort((targetA, targetB) => (Vector2.Distance(targetA.position, transform.position).CompareTo(Vector2.Distance(targetB.position, transform.position))) );
            
            // If the nearest entity detected is not within attack range, enter the Seek State
            if (Vector2.Distance(_entitiesDetected[0].position, transform.position) > _attackTargetRange)
                EnterSeekState();
        }
    }

    private void FiringGunSubState()
    {
        if (_currentFireTimer == _heldGun.FireRate)
        {
            // Don't fire if the entity somehow doesn't have a gun
            if (_heldGun == null)
                return;

            ShootBullet();

            // Reload immediately if the gun has no more bullets
            if (_heldGun.currentLoadedBullets <= 0)
            {
                _gunState = GunState.Reloading;
                return;
            }

            _currentFireTimer -= Time.deltaTime;
        }
        else if (_currentFireTimer > 0)
        {
            _currentFireTimer -= Time.deltaTime;
        }
        // If the fire timer runs out, either shoot again if it's auto, or bring it back to idle if it's not
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
            // Reset reload timer
            if (_isBoss)
                _currentReloadTimer = _heldGun.FireRate;
            else
                _currentReloadTimer = _heldGun.ReloadSpeed;

            // Reload
            _heldGun.currentLoadedBullets = _heldGun.MagSize;

            // Set the gun state back to Firing
            _gunState = GunState.Firing;
        }
    }

    private Vector2 FindRandomDestination()
    {

        // For wander state only
        _currentWanderTime = _wanderTime;

        // Pick a random point inside the target range
        Vector2 randomPoint = Vector2.zero;

        Collider2D hit;
        do
        {
            Vector2 randomDirection = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.right;
            randomPoint = new Vector2(transform.position.x, transform.position.y) + randomDirection * Random.Range(0, _rangeCollider.radius);
            hit = Physics2D.OverlapCircle(randomPoint, _bodyCollisionRadius, LayerMask.GetMask("Obstacles"));

        } while (hit != null);

        return randomPoint;
    }

    private void TraversePathTo(Vector2 destination)
    {
        Vector2 direction = destination - new Vector2(transform.position.x, transform.position.y);
        direction.Normalize();

        // Slowly interpolate velocity
        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, direction * _movementSpeed, _acceleration);
        RotateEntity(direction);

        // If there is an obstacle in the way, find another destination or enemy
        if (Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)) < 5f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _bodyCollisionRadius * 2, LayerMask.GetMask("Obstacles"));
            if (hit.collider != null)
            {
                if (_entitiesDetected.Count > 0)
                    _entitiesDetected.RemoveAt(0);
                else
                    _targetLocation = FindRandomDestination();
            }

        }

    }

    private void RotateEntity(Vector2 direction)
    {
        _body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), _acceleration);
    }


    private void ShootBullet()
    {
        _heldGun.currentLoadedBullets--;

        if (_heldGun.Type == GunType.Shotgun)
        {
            // Shoot 8 bullets if the current gun is a shotgun
            for (int i = 0; i < 8; i++)
            {
                float angle = Random.Range(-_heldGun.ShotgunArcAngle, _heldGun.ShotgunArcAngle);
                Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
                BulletScript newBullet = Instantiate(_heldGun.BulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
                newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(rotatedDirection), _heldGun.Damage, gameObject.GetComponent<EntityStatsScript>());
            }
        }
        else
        {
            BulletScript newBullet = Instantiate(_heldGun.BulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
            newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(Vector2.up), _heldGun.Damage, gameObject.GetComponent<EntityStatsScript>());
                
        }

        _muzzleParticles.Play();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + _body.right * _movementSpeed);

        //Gizmos.DrawWireSphere(_targetLocation, _bodyCollisionRadius);
    }
}

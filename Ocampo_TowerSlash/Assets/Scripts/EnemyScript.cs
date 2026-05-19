
using System.Collections.Generic;
using UnityEngine;

public enum ArrowColor
{
    Red,
    Yellow,
    Green
}

public class EnemyScript : MonoBehaviour
{

    [SerializeField] private float _enemyHealth;
    [SerializeField] private float _enemySpeed;

    [SerializeField] private SpriteRenderer _arrowRenderer;
    [SerializeField] private List<Sprite> _arrowSprites = new List<Sprite>();

    private SwipeDirection _direction = SwipeDirection.Right;
    private ArrowColor _arrowColor;
    private bool _isUndetectedByPlayer = true;
    

    public void Initialize()
    {
        _enemyHealth = Random.Range(10, 50);
        _enemySpeed = Random.Range(3, 5);

        _arrowColor = (ArrowColor)Random.Range(0, 3);
        _direction = (SwipeDirection)Random.Range(0, 4);

        SetArrowDirections();
    }

    public void GetDetectedByPlayer()
    {
        _isUndetectedByPlayer = false;

        if (_arrowColor == ArrowColor.Yellow)
        {
            _arrowRenderer.color = Color.green;
            StopAllCoroutines();
        }
    }

    public void KillEnemy()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3.down * _enemySpeed * Time.deltaTime), Time.deltaTime * _enemySpeed);
    }

    private void SetArrowDirections()
    {
        switch (_arrowColor)
        {
            case ArrowColor.Red:

                _arrowRenderer.sprite = _arrowSprites[ (int)Mathf.Repeat((int)_direction + 2, 3) ];
                _arrowRenderer.color = Color.red;
                break;


            case ArrowColor.Yellow:

                StartCoroutine(TimerScript.Instance.CO_ExecuteInSecondIntervals(ChangeArrowDirections, 0.5f));

                _arrowRenderer.color = Color.yellow;
                break;


            case ArrowColor.Green:

                _arrowRenderer.sprite = _arrowSprites[(int)_direction];
                _arrowRenderer.color = Color.green;
                break;

        }

    }

    private void ChangeArrowDirections()
    {
        
        _direction = (SwipeDirection) Mathf.Repeat((float)++_direction, 3f);
        _arrowRenderer.sprite = _arrowSprites[(int)_direction];
        
        Debug.Log("Changing");
    }

}

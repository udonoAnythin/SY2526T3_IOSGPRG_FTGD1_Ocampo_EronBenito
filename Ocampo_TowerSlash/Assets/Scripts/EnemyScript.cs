
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
    public SwipeDirection Direction
    {
        get => _direction;
    }

    [SerializeField] private float _enemySpeed = 1;

    [SerializeField] private SpriteRenderer _arrowRenderer;
    [SerializeField] private List<Sprite> _arrowSprites = new List<Sprite>();
    [SerializeField] private GameObject _arrowBackground;

    private SwipeDirection _direction = SwipeDirection.Right;
    private ArrowColor _arrowColor;

    //For Yellow Enemies Only
    private Coroutine _yellowCoroutine;
    //private bool _isUndetectedByPlayer = true;

    private void Start()
    {
        _arrowBackground.SetActive(false);
    }

    private void Update()
    {
        Move();
    }

    public void Initialize()
    {

        _arrowColor = (ArrowColor)Random.Range(0, 3);
        _direction = (SwipeDirection)Random.Range(0, 4);

        SetArrowDirections();

        //StartCoroutine(TimerScript.Instance.CO_ExecuteInCountdown(DeleteEnemyOffscreen, _deleteEnemyTimer));
    }

    public void GetDetectedByPlayer()
    {
        //_isUndetectedByPlayer = false;
        _arrowBackground.SetActive(true);

        if (_arrowColor == ArrowColor.Yellow)
        {
            _arrowRenderer.color = Color.green;
            StopCoroutine(_yellowCoroutine);
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
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

                Debug.Log($"{_direction} vs. {(int)(_direction + 2) % 4}");
                _arrowRenderer.sprite = _arrowSprites[ ((int)_direction + 2) % 4 ];
                _arrowRenderer.color = Color.red;
                break;


            case ArrowColor.Yellow:

                _yellowCoroutine = StartCoroutine(TimerScript.Instance.CO_ExecuteInSecondIntervals(ChangeArrowDirections, 0.5f));

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
        
        _direction = (SwipeDirection) ((int)++_direction % 4);
        _arrowRenderer.sprite = _arrowSprites[(int)_direction];
        
        //Debug.Log("Changing");
    }

    private void DeleteEnemyOffscreen()
    {
        Destroy(gameObject);
    }
}

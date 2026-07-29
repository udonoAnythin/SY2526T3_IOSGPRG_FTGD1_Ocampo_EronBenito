using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private float _updatePathTimer;

    [Header("Destroy State Variable")]
    [SerializeField] private GunData _heldGun;

    private float _currentWanderTime;
    private float _currentUpdatePathTimer;
    private float _currentFireTimer;
    private float _currentReloadTimer;

    private Stack<Vector2> _path;

    private void Awake()
    {
        _rangeCollider = GetComponent<CircleCollider2D>();
        _rangeCollider.radius = _entityDetectionRange;

        _currentWanderTime = _wanderTime;
        _currentUpdatePathTimer = _updatePathTimer;
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
        if (collision.GetComponent<EntityStatsScript>() != null)
        {
            _targets.Add(collision.transform);

            EnterSeekState();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<EntityStatsScript>() != null && _targets.Contains(collision.transform))
        {
            _targets.Remove(collision.transform);

            if (_targets.Count <= 0)
                EnterWanderState();
        }
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
        if (_currentUpdatePathTimer > 0)
        {
            _currentUpdatePathTimer -= Time.deltaTime;
        }
        else
        {
            Vector2 firstEndDest = _targets[0].position;
            _path = Pathfind(new Vector2(transform.position.x, transform.position.y), firstEndDest);

            _currentUpdatePathTimer = _updatePathTimer;
        }

        TraversePath();

        if (Vector2.Distance(_targets[0].position, transform.position) < _approachTargetRange)
            EnterDestroyState();
    }

    private void DestroyState()
    {

    }

    private void EnterWanderState()
    {
        Vector2 firstEndDest = FindRandomDestination();
        _path = Pathfind(new Vector2(transform.position.x, transform.position.y), firstEndDest);

        _currentState = EnemyStates.Wander;

        Debug.Log("Entered Wander");
    }

    private void EnterSeekState()
    {
        Vector2 firstEndDest = _targets[0].position;
        _path = Pathfind(new Vector2(transform.position.x, transform.position.y), firstEndDest);

        _currentState = EnemyStates.Seek;
    }

    private void EnterDestroyState()
    {

    }

    private Stack<Vector2> Pathfind(Vector2 startCoords, Vector2 endCoords)
    {
        // Nodes open for traversal
        List<PathfindingNode> traversableNodes = new List<PathfindingNode>();

        // Nodes already traversed
        HashSet<Vector2> traversedCoordinates = new HashSet<Vector2>();

        // Connections to previous nodes
        Dictionary<PathfindingNode, PathfindingNode> previousNode = new Dictionary<PathfindingNode, PathfindingNode>();

        // Final Path
        Stack<Vector2> finalPath = new Stack<Vector2>();

        // Set start node
        PathfindingNode startNode = new PathfindingNode(startCoords);
        traversableNodes.Add(startNode);

        // Set node to be traversed on
        PathfindingNode currentNode = startNode;

        // Pathfinding 
        while (currentNode.coordinates != endCoords)
        {

            UnityAction<float, float> CheckNeighbor = (float xCoordNeighbor, float yCoordNeighbor) =>
            {
                Vector2 neighborCoords = new Vector2(xCoordNeighbor, yCoordNeighbor);

                // Check first if neighbor node is traversable / has no obstacles
                if (Physics2D.OverlapCircle(neighborCoords, _bodyCollision.radius) == null && !traversedCoordinates.Contains(neighborCoords))
                {
                    // Check if the neighbor node is in the traversable list
                    int indexTraversableNeighbor = traversableNodes.FindIndex((node) => (node.coordinates == neighborCoords));
                    PathfindingNode neighborNode = null;

                    if (indexTraversableNeighbor == -1)
                    {
                        neighborNode = new PathfindingNode(neighborCoords);
                        traversableNodes.Add(neighborNode);

                        previousNode.Add(neighborNode, currentNode);

                        // Calculate new H Value
                        neighborNode.H = Vector2.Distance(neighborCoords, endCoords);
                    }
                    else
                        neighborNode = traversableNodes[indexTraversableNeighbor];

                    // Calculate G value
                    float newG = currentNode.G + Vector2.Distance(neighborCoords, currentNode.coordinates);

                    if (neighborNode.G == -1 || newG < neighborNode.G)
                        neighborNode.G = newG;

                    // Calc F Value
                    float newF = neighborNode.G + neighborNode.H;
                    if (neighborNode.F == -1 || newF < neighborNode.F)
                    {
                        neighborNode.F = newF;
                        previousNode[neighborNode] = currentNode;
                    }
                }
            };

            // Check all neighboring nodes
            for (float i = 0; i <= 360; i+=_pathfindingAngleIncrement)
            {
                Vector2 coordinates = Quaternion.Euler(0, 0, i) * Vector2.right;
                CheckNeighbor.Invoke(currentNode.coordinates.x + coordinates.x, currentNode.coordinates.y + coordinates.y);
            }
            
            // Add current coordinates to traversed
            traversedCoordinates.Add(currentNode.coordinates);
            traversableNodes.Remove(currentNode);

            if (traversableNodes.Count == 0) break;

            // Look for the node with the least F value
            traversableNodes.Sort((node, compareNode) => node.F.CompareTo(compareNode.F));
            currentNode = traversableNodes[0];
        }

        // Backtracking from the destination node
        PathfindingNode backtrackedNode = currentNode;

        while (backtrackedNode.coordinates != startCoords)
        {
            finalPath.Push(backtrackedNode.coordinates);
            backtrackedNode = previousNode[backtrackedNode];
        }
        return finalPath;
    }

    private Vector2 FindRandomDestination()
    {

        // For wander stat only
        _currentWanderTime = _wanderTime;

        // Pick a random point inside the target range
        Vector2 randomPoint = Vector2.zero;

        do
        {
            Vector2 randomDirection = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.right;
            randomPoint = randomDirection * Random.Range(0, _rangeCollider.radius);
        } while (Physics2D.OverlapCircle(randomPoint, _bodyCollision.radius, 0) == null);

        return randomPoint;
    }

    private void TraversePath()
    {
        Vector2 direction = _path.Peek() - new Vector2(transform.position.x, transform.position.y);
        direction.Normalize();

        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, direction * _movementSpeed, _acceleration);

        RotateEntity(_path.Peek());

        if (Vector2.Distance(_path.Peek(), new Vector2(transform.position.x, transform.position.y)) < 0.1)
        {
            _path.Pop();

            // if rat has completed path, make a new path
            if (_path.Count == 0)
            {
                Vector2 destination = FindRandomDestination();

                _path = Pathfind(new Vector2(transform.position.x, transform.position.y), destination);
            }
        }

    }

    private void RotateEntity(Vector2 target)
    {
        _body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg), _acceleration);
    }

}

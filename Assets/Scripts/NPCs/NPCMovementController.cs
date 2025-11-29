using StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovementController : MonoBehaviour, IPausable
{
    public float MoveSpeed = 5f;
    [Tooltip("Distance threshold to consider next node as goal")]
    [SerializeField] private float nextNodeTolerance;
    private NPCVisualController _visualController;
    [SerializeField] private Transform[] path; 
    private List<Node> _currentPath = new List<Node>();
    private int _targetNodeIndex = 0;

    private Action _onPathCompleteCallback;
    public Vector2 TargetPoint => _targetPoint;
    private Vector2 _targetPoint = Vector2.zero;
    private bool _isPaused = false;
    private bool _followsDefinedPath = false;
    private bool _isMovingForward = true;
    private int _definedIndex = 0;
    private bool _isMoving = false;

    private void Start()
    {
        _visualController = GetComponent<NPCVisualController>();
    }
    private void Update()
    {
        if(_isPaused) return;

        if(_followsDefinedPath && !_isMoving)
        {
            GoToNextPatrolNode();
        }

        if (_currentPath.Count > 0 && _targetNodeIndex < _currentPath.Count)
        {
            MoveAlongPath();
        } else
        {
            _visualController.NoMovement();
        }
    }

    public void StartPatrol()
    {
        if (path == null || path.Length < 2)
        {
            Debug.LogWarning("Cannot start patrol. Path is null or has fewer than 2 nodes.");
            return;
        }

        _followsDefinedPath = true;
        _definedIndex = 0;
        _isMovingForward = true;

        GoToNextPatrolNode();
    }

    public void StopPatrol()
    {
        _followsDefinedPath = false;
    }

    private void GoToNextPatrolNode()
    {
        if (!_followsDefinedPath || path == null || path.Length == 0)
        {
            _visualController.NoMovement();
            return;
        }

        if (_isMovingForward)
        {
            _definedIndex++;
            if (_definedIndex >= path.Length)
            {
                _isMovingForward = false;
                _definedIndex = path.Length - 2;
                if (_definedIndex < 0) _definedIndex = 0; 
            }
        }
        else 
        {
            _definedIndex--;
            if (_definedIndex < 0)
            {
                _isMovingForward = true;
                _definedIndex = 1;
                if (_definedIndex >= path.Length) _definedIndex = path.Length - 1; 
            }
        }

        Vector2 targetPos = path[_definedIndex].position;
        GoToPosition(targetPos, () => {
            _isMoving = false;
        });
    }

    public void SetTargetPoint(Vector2 point)
    {
        _targetPoint = point;
    }

    public Vector2 GenerateNewPointInRange(Vector2 referencePoint, float range)
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * range;

        return referencePoint + randomOffset;
    }

    public void GoToPosition(Vector2 targetPosition, Action onPathComplete)
    {
        _onPathCompleteCallback = onPathComplete;

        Node startNode = AStarManager.Instance.FindNearestNode(transform.position);

        Node endNode = AStarManager.Instance.FindNearestNode(targetPosition);

        if (startNode == null || endNode == null)
        {
            _onPathCompleteCallback?.Invoke();
            _onPathCompleteCallback = null;
            _currentPath.Clear();
            return;
        }

        List<Node> newPath = AStarManager.Instance.GeneratePath(startNode, endNode);

        if (newPath != null)
        {
            _currentPath = newPath;
            _targetNodeIndex = 0;
            _isMoving = true;
            //Debug.Log($"Path generated with {newPath.Count} steps.");
        }
        else
        {
            Debug.LogWarning("No path found between start and end.");
            onPathComplete?.Invoke();
            _onPathCompleteCallback?.Invoke();
            _onPathCompleteCallback = null;
            _currentPath.Clear();
        }
    }

    private void MoveAlongPath()
    {
        Vector2 targetPos = _currentPath[_targetNodeIndex].transform.position;

        Vector2 direction = targetPos - (Vector2)transform.position;
        _visualController.DetermineCardinalDirection(direction.normalized);
        float distance = direction.magnitude;

        if (distance <= nextNodeTolerance)
        {
            _targetNodeIndex++;

            if (_targetNodeIndex >= _currentPath.Count)
            {
                _currentPath.Clear();
                transform.position = targetPos;
                _onPathCompleteCallback?.Invoke();
                _onPathCompleteCallback = null;
            }
        }
        else
        {
            Vector2 movement = direction.normalized * MoveSpeed * Time.deltaTime;
            transform.position = (Vector2)transform.position + movement;
        }
    }

    public void InterrumptPath()
    {
        _currentPath.Clear();
        _onPathCompleteCallback = null;
    }

    public List<Node> GetCurrentPath()
    {
        return _currentPath;
    }

    public void OnPause()
    {
        _isPaused = true;
    }

    public void OnResume()
    {
        _isPaused = false;
    }
}

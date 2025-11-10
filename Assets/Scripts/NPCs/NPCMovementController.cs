using StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovementController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    [Tooltip("Distance threshold to consider next node as goal")]
    [SerializeField] private float nextNodeTolerance;
    private NPCVisualController _visualController;

    private StateController _stateController;
    private List<Node> _currentPath = new List<Node>();
    private int _targetNodeIndex = 0;

    private Action _onPathCompleteCallback;
    public Vector2 TargetPoint => _targetPoint;
    private Vector2 _targetPoint = Vector2.zero;


    private void Start()
    {
        _stateController = GetComponent<StateController>();
        _visualController = GetComponent<NPCVisualController>();
    }
    private void Update()
    {
        if (_currentPath.Count > 0 && _targetNodeIndex < _currentPath.Count)
        {
            MoveAlongPath();
        } else
        {
            _visualController.NoMovement();
        }
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
            Debug.LogWarning("Could not find start or end node for pathfinding.");
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
            Debug.Log($"Path generated with {newPath.Count} steps.");
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
                //_stateController.CurrentState.FinishState();
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
    }

    public List<Node> GetCurrentPath()
    {
        return _currentPath;
    }
}

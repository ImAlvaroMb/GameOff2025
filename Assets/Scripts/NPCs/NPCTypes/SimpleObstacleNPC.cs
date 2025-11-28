using StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class SimpleObstacleNPC : MonoBehaviour // this class shouldnt exist, i would use the npc existing states to create a npc variable that doesnt do all the actions.NO TIME
{
    public float MoveSpeed = 5f;
    [SerializeField] private float nextNodeTolerance;

    private List<Node> _currentPath = new List<Node>();
    private int _targetNodeIndex = 0;
    [SerializeField] private GameObject kickNPCPos;
    [SerializeField] private NPCVisualController _visualController;
    private int _NPCLayerID;
    private Action _onPathCompleteCallback;

    private Vector2 _targetPoint = Vector2.zero;

    private Vector2 _initialPos;

    private void Start()
    {
        _initialPos = transform.position;
        _NPCLayerID = LayerMask.NameToLayer("NPC");
    }

    private void Update()
    {
        if (_currentPath.Count > 0 && _targetNodeIndex < _currentPath.Count)
        {
            MoveAlongPath();
        }
    }

    #region Movement
    public void SetTargetPoint(Vector2 point)
    {
        _targetPoint = point;
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
        _visualController.DetermineCardinalDirection(direction);
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

    #endregion

    public Vector2 GetInitialPos()
    {
        return _initialPos; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == _NPCLayerID && collision.gameObject.CompareTag("NPC"))
        {
            if(!collision.gameObject.GetComponentInParent<NPCAwarness>().IsTeacher())
            {
                collision.gameObject.GetComponentInParent<StateController>().CurrentState.FinishState();
                collision.gameObject.GetComponentInParent<NPCMovementController>().SetTargetPoint(kickNPCPos.transform.position);
                collision.gameObject.GetComponentInParent<NPCController>().SetCurrentAction(NPCActions.PATROL);
                _visualController.ActivateSpeechBubble(() => { });
                //AlertSystemController.Instance.SendAlert("YOU CANT PASS, MOVE THIS NPC TO ADVANCE", 2f);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Enums;
using Utilities;

public class NPCController : MonoBehaviour
{
    [SerializeField] private List<AIAction> actionsProbabilities = new List<AIAction>();
    public NPCActions CurrentAction => _currentAction;
    public bool DrawGizmos;
    private NPCActions _currentAction;
    private float _currentTotalProbability;

    [Header("Patrol")]
    public Vector2 OriginalPosition => _originalPosition;
    private Vector2 _originalPosition;
    public float RangeToPatrol => rangeToPatrol;
    [SerializeField] private float rangeToPatrol;

    [Header("Object Interaction")]
    public BaseInteractable CurrentInteractable => _currentInteractable;
    private BaseInteractable _currentInteractable = null;

    [Header("Being Controller")]
    public bool IsFullyControlled => _isFullyControlled;
    private bool _isFullyControlled = false;
    private ITimer _beingControlledTimer;
    public bool IsInInfluenceArea => _isInInfluenceArea;
    private bool _isInInfluenceArea = false;
    public bool IsBeingControlled => _isBeingControlled;
    private bool _isBeingControlled = false;
    private void OnValidate()
    {
        _currentTotalProbability = actionsProbabilities.Sum(item => item.Probability);

        const float target = 100f;
        const float tolerance = 0.01f; 

        if (Mathf.Abs(_currentTotalProbability - target) > tolerance)
        {
            Debug.LogError($"Validation Error on {gameObject.name}: Action Probabilities must sum to 100. Current sum is {_currentTotalProbability:F2}%.");
        }
        else
        {
            Debug.Log($"Validation Success on {gameObject.name}: Total probability is exactly 100%.");
        }
    }

    private void Start()
    {
        _originalPosition = transform.position;
    }

    #region Controlled

    public void OnClicked()
    {
        Debug.Log($"ClickedOn {gameObject.name} NPC");
        if(!_isBeingControlled && _isInInfluenceArea)
        {
            _isBeingControlled = true;
        } else if(_isBeingControlled && _isInInfluenceArea)
        {
            _isBeingControlled = false;
        }
    }

    public void SetIsInAreaOfInfluence(bool value)
    {
        _isInInfluenceArea = value;
    }

    public void SetIsBeingControlled(bool value)
    {
        _isBeingControlled = value;
    }

    public void SetIsFullyControlled(bool value)
    {
        _isFullyControlled = value;
    }

    public void DisruptBeingControlled()
    {
        if (_beingControlledTimer != null)
        {
            _beingControlledTimer.StopTimer();
            _beingControlledTimer = null;
            _isBeingControlled = false;
        }
    }

    #endregion

    #region Action choosing
    public void ResetAction()
    {
        _currentAction = NPCActions.NONE;
    }

    public void DecideNextAction()
    {
        ReevaluateProbabilities();
    }

    private void ReevaluateProbabilities()
    {
        ChooseAction();
    }

    private void ChooseAction()
    {
        float randomNumber = Random.Range(0f, _currentTotalProbability);
        Debug.Log(randomNumber);
        float cumulativeWeight = 0f;

        foreach (var action in actionsProbabilities)
        {
            cumulativeWeight += action.Probability;

            if(randomNumber <= cumulativeWeight)
            {
                _currentAction = action.Action;
                Debug.Log(_currentAction);
                break;
            }
        }
    }

    public void SetCurrentAction(NPCActions action)
    {
        _currentAction = action;
    }
    #endregion

    #region Object Interaction

    public void SetCurrentInteractable(BaseInteractable interactable)
    {
        _currentInteractable = interactable;
    }

    public void RemoveCurrentInteractable()
    {
        _currentInteractable = null;
    }

    #endregion

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        if(DrawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, rangeToPatrol);
        }
        
    }
}

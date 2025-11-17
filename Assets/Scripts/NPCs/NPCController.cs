using Enums;
using StateMachine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Utilities;

public class NPCController : MonoBehaviour
{
    [SerializeField] private List<AIAction> actionsProbabilities = new List<AIAction>();
    public NPCActions CurrentAction => _currentAction;
    public bool DrawGizmos;
    private NPCActions _currentAction;
    private float _currentTotalProbability;
    private NPCVisualController _visualController;
    [Header("Area of Influence")]
    private HashSet<RangeOfInfluenceObject> _activeAreas = new HashSet<RangeOfInfluenceObject>();

    [Header("Patrol")]
    public Vector2 OriginalPosition => _originalPosition;
    private Vector2 _originalPosition;
    public float RangeToPatrol => rangeToPatrol;
    [SerializeField] private float rangeToPatrol;

    [Header("Object Interaction")]
    public BaseInteractable CurrentInteractable => _currentInteractable;
    private BaseInteractable _currentInteractable = null;
    public NPCController OtherCurrentNPC => _otherCurrentNPC;
    private NPCController _otherCurrentNPC = null;

    [Header("Talking To NPC")]
    public TalkType TalkType => _talkType;
    private TalkType _talkType = TalkType.NONE;

    [Header("Waving To NPC")]
    public NPCWaveType WaveType => _waveType;
    private NPCWaveType _waveType = NPCWaveType.NONE;
    [SerializeField] private float waveDistance = 3f;

    [Header("Being Controller")]
    public bool IsFullyControlled => _isFullyControlled;
    private bool _isFullyControlled = false;
    private ITimer _beingControlledTimer;
    public bool IsInInfluenceArea => _isInInfluenceArea;
    private bool _isInInfluenceArea = false;
    public bool IsBeingControlled => _isBeingControlled;
    private bool _isBeingControlled = false;
    private float _currentProportionalInfluence;
    [SerializeField] private float decayDuration = 10f; //time in seconds to go from fully controlled to loosing control of the NPC
    [SerializeField] private float recoveryDuration = 10f; //time in seconds to go from fully controlled to loosing control of the NPC
    private ITimer _influenceTimer = null;
    private string _timerID;
    private bool _isInfluenceAtFull = false;
    private bool _canCheckInfluence = false;

    [Header("CameraFollow")]
    public GameObject CameraReferece;
    
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
        _visualController = GetComponent<NPCVisualController>();
    }

    private void OnDestroy()
    {
        if(_influenceTimer != null)
        {
            TimerSystem.Instance.StopTimer(_timerID);
        }
    }

    private void Update()
    {
        if(_isFullyControlled && _canCheckInfluence)
        {
            if (_isInInfluenceArea)
            {
                HandleInfluenceRecovery();
            }
            else
            {
                HandleInfluenceDecay();
            }
            _visualController.UpdateInfluenceMeterImage(_currentProportionalInfluence);
        }
    }
    #region Range Of Influence

    public void OnAreaEntered(RangeOfInfluenceObject area)
    {
        bool isNew = _activeAreas.Add(area);
        if(isNew && _activeAreas.Count == 1)
        {
            _isInInfluenceArea = true;
        }
    }

    public void OnAreaExit(RangeOfInfluenceObject area)
    {
        bool wasRemoved = _activeAreas.Remove(area);    
        if(wasRemoved && _activeAreas.Count == 0)
        {
            _isInInfluenceArea = false;
        }
    }

    #endregion

    #region Controlled

    private void HandleInfluenceRecovery()
    {
        if (_isInfluenceAtFull)
        {
            StopActiveTimer();
            return;
        }

        // If already recovering, let the current timer run
        if (_influenceTimer != null && _influenceTimer.GetData().Direction == TimerDirection.INCREASE)
        {
            return;
        }

        StopActiveTimer();
        float startCurrentTime = _currentProportionalInfluence * recoveryDuration;

        _influenceTimer = TimerSystem.Instance.CreateTimer(recoveryDuration, TimerDirection.INCREASE, onTimerIncreaseComplete: () =>
        {
            _isInfluenceAtFull = true;
            _currentProportionalInfluence = 1f;
            StopActiveTimer(); 
        }, onTimerIncreaseUpdate: (progress) =>
        {
            _currentProportionalInfluence = progress / recoveryDuration;
        });

        _timerID = _influenceTimer.GetData().ID;

        // Set the starting position and direction
        TimerSystem.Instance.ModifyTimer(
            _influenceTimer,
            newCurrentTime: startCurrentTime,
            newDirection: TimerDirection.INCREASE,
            isRunning: true
        );
    }

    private void HandleInfluenceDecay()
    {
        if (_influenceTimer != null && _influenceTimer.GetData().Direction == TimerDirection.DECREASE)
        {
            return;
        }

        StopActiveTimer();
        _isInfluenceAtFull = false; 
        float startCurrentTime = _currentProportionalInfluence * decayDuration;

        _influenceTimer = TimerSystem.Instance.CreateTimer(decayDuration, TimerDirection.DECREASE, onTimerDecreaseComplete: () =>
        {
            _isFullyControlled = false;
            _canCheckInfluence = false;
            StopActiveTimerAndZero();
        }, onTimerDecreaseUpdate: (progress) =>
        {
            _currentProportionalInfluence = progress / decayDuration;
        });

        _timerID = _influenceTimer.GetData().ID;

        TimerSystem.Instance.ModifyTimer(
            _influenceTimer,
            newCurrentTime: startCurrentTime,
            newDirection: TimerDirection.DECREASE,
            isRunning: true
        );
    }

    private void StopActiveTimer()
    {
        if (_influenceTimer != null)
        {
            if (!string.IsNullOrEmpty(_timerID))
            {
                TimerSystem.Instance.StopTimer(_timerID);
            }
            _influenceTimer = null;
            _timerID = null;
        }
    }

    private void StopActiveTimerAndZero()
    {
        StopActiveTimer();
        _isInfluenceAtFull = false;
        _currentProportionalInfluence = 0f;
        _visualController.UpdateInfluenceMeterImage(0f);
    }

    public void HasBeenFullyControlled()
    {
        StopActiveTimer();
        _isFullyControlled = true;
        _currentProportionalInfluence = 1f;
        _isInfluenceAtFull = true;
        _visualController.UpdateInfluenceMeterImage(_currentProportionalInfluence);
        TimerSystem.Instance.CreateTimer(0.15f, onTimerDecreaseComplete: () =>
        {
            _currentProportionalInfluence = 1f; 
            _canCheckInfluence = true;
        }); //small delay to avoid creating a non necessary timer on update
        
    }


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
        float cumulativeWeight = 0f;

        foreach (var action in actionsProbabilities)
        {
            cumulativeWeight += action.Probability;

            if(randomNumber <= cumulativeWeight)
            {
                _currentAction = action.Action;
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

    #region NPC Interactions
    public void SetTalkType(TalkType talkType)
    {
        _talkType = talkType;
    } 

    public void SetOtherNPCReference(NPCController otherNPC)
    {
        _otherCurrentNPC = otherNPC;
    }

    public void RemoveCurrentOtherNPCReference()
    {
        _otherCurrentNPC = null;
    }

    public void SetWaveType(NPCWaveType waveType)
    {
        _waveType = waveType;
    }

    public float GetWaveDistance()
    {
        return waveDistance;
    }

    #endregion

    public void OnHover(bool isCurrentlyControlledNPC) 
    {
        //Debug.Log($"Hover {gameObject.name}");
        _visualController.OnHovered(isCurrentlyControlledNPC);
    }

    public void OnStopHover()
    {
        _visualController.OnStopHovering();
    }
    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void StartCameraFollow()
    {
        CameraReferece.SetActive(true);
    }

    public void StopCameraFollow()
    {
        CameraReferece.SetActive(false);
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

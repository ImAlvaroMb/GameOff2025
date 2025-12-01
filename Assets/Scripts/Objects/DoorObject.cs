using Enums;
using StateMachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
public class DoorObject : MonoBehaviour
{
    [SerializeField] private float animDuration;
    [SerializeField] private float targetYRotation = 75f;
    [SerializeField] private bool checksForNpc = false;
    [SerializeField] private bool needsKey = false;
    [SerializeField] private Transform kickPos;
    [SerializeField] private string message;

    [Header("New detection")]
    [SerializeField] private Transform doorCentralPoint;
    [SerializeField] private float influenceAreaRadius = 3f; 
    [SerializeField] private List<NPCController> _targetNPCs; 
    private HashSet<NPCController> _npcInsideArea = new HashSet<NPCController>();
    

    public UnityEvent OnOpen;
    private int _NPCLayerID;
    private ITimer _timer;
    private bool _hasKey = false;
    private bool _doorIsOpen = false;

    private void Start()
    {
        _NPCLayerID = LayerMask.NameToLayer("NPC");
        FindAllNPC();
    }

    [ContextMenu("FindAllNpc")]
    private void FindAllNPC()
    {
        _targetNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (_targetNPCs == null) return;

        foreach (NPCController npcController in _targetNPCs)
        {
            if (npcController == null) continue;

            if (Vector2.Distance(npcController.transform.position, doorCentralPoint.position) < influenceAreaRadius)
            {
                if (_npcInsideArea.Add(npcController)) 
                {
                    OnAreaEntered(npcController);
                }
                
            }
            else
            {
                if (_npcInsideArea.Remove(npcController)) 
                {
                    OnAreaExit(npcController);
                }
            }
        }
    }

    private void OnAreaEntered(NPCController npcController)
    {
        if (checksForNpc)
        {

            NPCAwarness npcAwareness = npcController.GetComponent<NPCAwarness>();

            if (needsKey && _hasKey && npcAwareness != null && !npcAwareness.IsTeacher())
            {
                OpenDoor();
            }
            else if (needsKey && !_hasKey)
            {
                AlertSystemController.Instance.SendAlert(message, 2f);
            }

            if (!needsKey && npcAwareness.IsTeacher())
            {
                OpenDoor();
            } else if(!needsKey && !npcAwareness.IsTeacher())
            {
                StateController stateController = npcController.GetComponent<StateController>();
                npcController.OtherCurrentNPC?.GetComponent<StateController>()?.CurrentState.FinishState();
                stateController?.CurrentState.FinishState();
            }

        }
        else
        {
            if (!_doorIsOpen)
            {
                StateController stateController = npcController.GetComponent<StateController>();
                npcController.OtherCurrentNPC?.GetComponent<StateController>()?.CurrentState.FinishState();
                stateController?.CurrentState.FinishState();
            }            
        }
    }

    private void OnAreaExit(NPCController npcController)
    {
        if (checksForNpc)
        {
            // CloseDoor(); 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(doorCentralPoint.transform.position, influenceAreaRadius);
    }

    public void CheckCanOpenDoor()
    {
        if(needsKey && _hasKey)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            targetYRotation,
            transform.rotation.eulerAngles.z);

        if(_timer == null)
        {
            _timer = TimerSystem.Instance.CreateTimer(animDuration, TimerDirection.INCREASE, onTimerIncreaseComplete: () =>
            {
                transform.rotation = targetRotation;
                _timer = null;
                OnOpen?.Invoke();
            }, onTimerIncreaseUpdate: (progress) =>
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            });
        }

        _doorIsOpen = true;
    }

    public void CloseDoor()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            0f,
            transform.rotation.eulerAngles.z);

        if (_timer == null)
        {
            _timer = TimerSystem.Instance.CreateTimer(animDuration, TimerDirection.INCREASE, onTimerIncreaseComplete: () =>
            {
                transform.rotation = targetRotation;
                _timer = null;
            }, onTimerIncreaseUpdate: (progress) =>
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            });
        }
    }

    public void SetHasKey(bool value)
    {
        _hasKey = value;
    }
}

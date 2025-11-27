using Enums;
using StateMachine;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
public class DoorObject : MonoBehaviour
{
    [SerializeField] private float animDuration;
    [SerializeField] private float targetYRotation = 75f;
    [SerializeField] private bool checksForNpc = false;
    [SerializeField] private Transform kickPos;

    public UnityEvent OnOpen;
    private int _NPCLayerID;
    private ITimer _timer;

    private void Start()
    {
        _NPCLayerID = LayerMask.NameToLayer("NPC");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (checksForNpc)
        {
            if (collision.gameObject.CompareTag("NPC") && collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                OpenDoor();
            }
        } else
        {
            if (collision.gameObject.layer == _NPCLayerID && collision.gameObject.CompareTag("NPC"))
            {
                collision.gameObject.GetComponentInParent<NPCController>().OtherCurrentNPC?.GetComponent<StateController>().CurrentState.FinishState();
                collision.gameObject.GetComponentInParent<StateController>().CurrentState.FinishState();
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(checksForNpc)
        {
            if (collision.gameObject.CompareTag("NPC") && collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                CloseDoor();
            }
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
}

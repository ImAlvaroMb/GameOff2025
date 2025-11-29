using UnityEngine;
using Utilities;
using Enums;
using UnityEngine.UI;
using StateMachine;

public class SuspiciousNPC : MonoBehaviour
{
    [SerializeField] private Image alertIndicator;
    [SerializeField] public FrogInteractable frogInteractable;
    [SerializeField] private float timeToAlert = 5f;
    [SerializeField] private float alertRadius = 3f;
    private ITimer _timer;
    private NPCController _targetToCheck;

    public void SetCurrentTargetToCheck(NPCController newTarget)
    {
        _targetToCheck = newTarget;
    }

    private void FixedUpdate()
    {
        if(_targetToCheck != null)
        {
            if(IsTargetOnDistance())
            {
                if(_timer == null)
                {
                    _timer = TimerSystem.Instance.CreateTimer(timeToAlert, TimerDirection.INCREASE, onTimerIncreaseComplete: () =>
                    {
                        _timer = null;
                        //LevelController.Instance.OnLose.Invoke();
                        _targetToCheck.gameObject.GetComponent<StateController>().CurrentState.FinishState();
                        _targetToCheck.SetCurrentInteractable(frogInteractable);
                        _targetToCheck.SetCurrentAction(NPCActions.DO_OBJECT_INTERACTION);
                    }, onTimerIncreaseUpdate: (progress) =>
                    {
                        float t = progress / timeToAlert;
                        alertIndicator.fillAmount = t;
                    });
                }
            } else
            {
                alertIndicator.fillAmount = 0;
                _timer?.StopTimer();
                _timer = null;
            }
            
        } else
        {
            alertIndicator.fillAmount = 0f;
            _timer?.StopTimer();
            _timer = null;
        }
    }

    private bool IsTargetOnDistance()
    {
        if (Vector2.Distance(transform.position, _targetToCheck.transform.position) < alertRadius)
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}

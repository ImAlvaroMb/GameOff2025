using UnityEngine;
using Utilities;
using Enums;
public class DoorObject : MonoBehaviour
{
    [SerializeField] private float animDuration;
    [SerializeField] private float targetYRotation = 75f;
    [SerializeField] private bool checksForNpc = false;

    private ITimer _timer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!checksForNpc) return;
        if(collision.gameObject.CompareTag("NPC") && collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!checksForNpc) return;
        if (collision.gameObject.CompareTag("NPC") && collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            CloseDoor();
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

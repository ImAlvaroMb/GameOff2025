using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

public class TriggerSomethingForAPeriodOfTimeInteractable : BaseInteractable
{
    public GameObject UIContainer;
    public Image fillImage;
    [SerializeField] private float actionDuration = 12f;
    public UnityEvent OnTimerFinished;


    private ITimer _currentTimer;

    

    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);

        if(_currentTimer == null)
        {
            UIContainer.SetActive(true);
            fillImage.fillAmount = 1f;
            _currentTimer = TimerSystem.Instance.CreateTimer(actionDuration, onTimerDecreaseComplete: () =>
            {
                OnTimerFinishedInvoke();
                _currentTimer = null;
            }, onTimerDecreaseUpdate: (progress) =>
            {
                fillImage.fillAmount = progress;
            });
        }
    }

    

    private void OnTimerFinishedInvoke()
    {
        UIContainer.SetActive(false);
        OnTimerFinished.Invoke();
    }
}

using UnityEngine;
using Utilities;
using Enums;
[CreateAssetMenu(menuName ="State/BeingControlled")]
public class NPCBeingControlledState : NPCBaseState
{
    public float TakeControlDuration;
    private ITimer _currentTimer;
    public override void OnEnter()
    {
        base.OnEnter();
        _currentTimer = TimerSystem.Instance.CreateTimer(TakeControlDuration, TimerDirection.INCREASE,onTimerIncreaseComplete: () =>
        {
            FinishState();
        }, onTimerIncreaseUpdate: (progress) =>
        {
            _visualController.UpdateInfluenceMeterImage(progress / TakeControlDuration);
        });
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void OnExit()
    {
        base.OnExit();
        _currentTimer.StopTimer();
        _currentTimer = null;
        _visualController.UpdateInfluenceMeterImage(0f);
    }

}

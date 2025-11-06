using UnityEngine;
using Utilities;
using StateMachine;
[CreateAssetMenu(menuName ="State/WaitState")]
public class NPCWaitState : NPCBaseState
{
    public float MinWaitTime;
    public float MaxWaitTime;

    public override void OnEnter()
    {
        base.OnEnter();
        float duration = Random.Range(MinWaitTime, MaxWaitTime);

        TimerSystem.Instance.CreateTimer(duration, onTimerDecreaseComplete: () =>
        {
            _isDone = true;
        });
    }
}

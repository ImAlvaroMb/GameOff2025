using StateMachine;
using UnityEngine;

[CreateAssetMenu(menuName ="State/SaySomething")]
public class NPCSaySomethingState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _visualController.ActivateSpeechBubble(() =>
        {
            _isDone = true;
        });
    }

    public override void OnExit()
    {
        base.OnExit();
    }


}

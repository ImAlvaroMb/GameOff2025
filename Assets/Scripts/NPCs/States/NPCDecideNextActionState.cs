using UnityEngine;
using StateMachine;
[CreateAssetMenu(menuName ="State/DecideNextAction")]
public class NPCDecideNextActionState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _controller.DecideNextAction();
    }
}

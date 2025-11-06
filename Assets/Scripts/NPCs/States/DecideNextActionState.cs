using UnityEngine;
using StateMachine;
public class DecideNextActionState : NPCBaseState
{
    public override void OnEnter()
    {
        _currentAction = Enums.NPCActions.PATROL;
    }
    public override void OnExit()
    {
        
    }
    public override void FinishState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}

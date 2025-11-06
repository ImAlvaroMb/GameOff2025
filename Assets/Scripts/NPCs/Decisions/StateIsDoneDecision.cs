using UnityEngine;
using StateMachine;
[CreateAssetMenu(menuName ="Decision/StateIsDone")]
public class StateIsDoneDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        return stateController.CurrentState.IsDone;
    }
}

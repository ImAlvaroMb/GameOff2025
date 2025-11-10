using UnityEngine;
using StateMachine;
[CreateAssetMenu(menuName ="Decision/IsFullyControlled")]
public class IsFullyControlledDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        return stateController.GetComponent<NPCController>().IsFullyControlled;
    }
}

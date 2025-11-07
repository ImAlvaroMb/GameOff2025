using UnityEngine;
using StateMachine;
[CreateAssetMenu(menuName ="Decision/IsBeingControlled")]
public class IsBeingControlledDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
         return stateController.GetComponent<NPCController>().IsBeingControlled;
    }
}

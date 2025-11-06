using UnityEngine;
using StateMachine;
[CreateAssetMenu(menuName ="Decision/Patrol")]
public class PatrolDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        if(stateController.GetComponent<NPCController>().CurrentAction == Enums.NPCActions.PATROL)
        {
            return true;
        } else
        {
            return false;
        }
    }
}

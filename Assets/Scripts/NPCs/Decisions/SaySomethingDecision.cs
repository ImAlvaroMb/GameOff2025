using UnityEngine;
using StateMachine;
[CreateAssetMenu(menuName ="Decision/SaySomething")]
public class SaySomethingDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        if(stateController.gameObject.GetComponent<NPCController>().CurrentAction == Enums.NPCActions.SAY_SOMETHING)
        {
            return true;
        } else
        {
            return false;
        }
    }
}

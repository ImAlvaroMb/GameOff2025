using UnityEngine;
using StateMachine;
using Enums;
[CreateAssetMenu(menuName ="Decision/InteractWithObjDecision")]
public class InteractWithObjDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        if(stateController.GetComponent<NPCController>().CurrentAction == NPCActions.DO_OBJECT_INTERACTION)
        {
            return true;
        } else
        {
            return false;
        }
        
    }
}

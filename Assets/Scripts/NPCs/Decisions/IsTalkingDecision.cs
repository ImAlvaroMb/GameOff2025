using StateMachine;
using Enums;
using UnityEngine;
[CreateAssetMenu(menuName ="Decision/IsTalking")]
public class IsTalkingDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        if(stateController.GetComponent<NPCController>().TalkType != TalkType.NONE || stateController.GetComponent<NPCController>().CurrentAction == NPCActions.TALK_TO_NPC)
        {
            return true;
        } else
        {
            return false;
        }
    }
}

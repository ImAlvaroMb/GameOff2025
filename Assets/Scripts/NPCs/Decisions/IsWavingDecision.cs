using UnityEngine;
using StateMachine;
using Enums;
[CreateAssetMenu(menuName ="Decision/IsWaving")]
public class IsWavingDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        if(stateController.GetComponent<NPCController>().CurrentAction == NPCActions.WAVE || stateController.GetComponent<NPCController>().WaveType != NPCWaveType.NONE)
        {
            return true;
        } else
        {
            return false;   
        }
    }
}

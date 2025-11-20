using UnityEngine;
using Enums;
using StateMachine;
[CreateAssetMenu(menuName ="State/WaveToNPC")]
public class NPCWaveToNPCState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _visualController.OnAction(NPCActions.WAVE);
        if(_controller.WaveType == NPCWaveType.WAVER)
        {
            // wave animation
            if(_controller.OtherCurrentNPC != null) //user fprced this state
            {
                if(IsOnDistanceToTarget())
                {
                    _controller.OtherCurrentNPC.SetOtherNPCReference(_controller);
                    _controller.OtherCurrentNPC.SetWaveType(NPCWaveType.WAVE_RECEIVER);
                    // just wait
                } else //notify other npc to finish state and finish own state
                {
                    _controller.OtherCurrentNPC.gameObject.GetComponent<StateController>().CurrentState.FinishState();
                    FinishState();
                }
            } else
            {
                _controller.SetOtherNPCReference(_awarnessController.GetNPC());
                if(_controller.OtherCurrentNPC == null)
                {
                    FinishState();
                }
                else
                {
                   if(IsOnDistanceToTarget())
                    {
                        _controller.OtherCurrentNPC.SetOtherNPCReference(_controller);
                        _controller.OtherCurrentNPC.SetWaveType(NPCWaveType.WAVE_RECEIVER);
                    }
                }
            } 
        } else
        {
            if(_controller.OtherCurrentNPC != null)
            {
                GoToNPC();
            } else
            {
                FinishState();
            }
        }
        
    }

    private void GoToNPC()
    {
        _movementController.GoToPosition(_controller.OtherCurrentNPC.transform.position, () =>
        {
            //wave anim
            _controller.OtherCurrentNPC.GetComponent<StateController>().CurrentState.FinishState();
            _isDone = true;
        });
    }

    private bool IsOnDistanceToTarget()
    {
        if(Vector2.Distance(_controller.gameObject.transform.position, _controller.OtherCurrentNPC.transform.position) > _controller.GetWaveDistance())
        {
            return false;
        } else
        {
            return true;
        }
    }



    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller.SetOtherNPCReference(null);
        _controller.SetWaveType(NPCWaveType.NONE);
    }
}

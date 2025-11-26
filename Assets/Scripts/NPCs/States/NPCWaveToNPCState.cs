using UnityEngine;
using Enums;
using StateMachine;
[CreateAssetMenu(menuName ="State/WaveToNPC")]
public class NPCWaveToNPCState : NPCBaseState
{

    public override void OnEnter()
    {
        base.OnEnter();
        

        /*if(_controller.WaveTargets.Length > 0) 
        {
            if(IsTargetCorrect())
            {
                HandleGoingToTarget();
            } else
            {
                FinishState();
            }
        } else
        {
            HandleGoingToTarget();
        }*/

        HandleGoingToTarget();
    }


    private void HandleGoingToTarget()
    {
        _visualController.OnAction(NPCActions.WAVE);
        if (_controller.WaveType == NPCWaveType.WAVER)
        {
            // wave animation
            if (_controller.OtherCurrentNPC != null) //user fprced this state
            {
                if (IsOnDistanceToTarget())
                {
                    _controller.OtherCurrentNPC.SetOtherNPCReference(_controller);
                    _controller.OtherCurrentNPC.SetWaveType(NPCWaveType.WAVE_RECEIVER);
                    // just wait
                }
                else //notify other npc to finish state and finish own state
                {
                    _controller.OtherCurrentNPC.gameObject.GetComponent<StateController>().CurrentState.FinishState();
                    FinishState();
                }
            }
            else
            {
                _controller.SetOtherNPCReference(_awarnessController.GetNPC());
                if (_controller.OtherCurrentNPC == null)
                {
                    FinishState();
                }
                else
                {
                    if (IsOnDistanceToTarget())
                    {
                        _controller.OtherCurrentNPC.SetOtherNPCReference(_controller);
                        _controller.OtherCurrentNPC.SetWaveType(NPCWaveType.WAVE_RECEIVER);
                    }
                }
            }
        }
        else
        {
            if (_controller.OtherCurrentNPC != null)
            {
                if(_controller.WaveTargets.Length > 0)
                {
                    if (IsTargetCorrect())
                    {
                        GoToNPC();
                    }
                    else
                    {
                        FinishState();
                    }
                } else
                {
                    GoToNPC();
                }
                
            }
            else
            {
                FinishState();
            }
        }
    }

    private void GoToNPC()
    {
        Vector2 position = _controller.OtherCurrentNPC.GetRandomValidInteractionPoint();
        _movementController.GoToPosition(position, () =>
        {
            Vector2 direction = stateController.gameObject.transform.position - _controller.OtherCurrentNPC.transform.position;
            Vector2 direction2 = _controller.OtherCurrentNPC.transform.position - stateController.gameObject.transform.position; 
            _controller.OtherCurrentNPC.gameObject.GetComponent<NPCVisualController>().DetermineCardinalDirection(direction);
            _visualController.DetermineCardinalDirection(direction2);
            _controller.OtherCurrentNPC.GetComponent<StateController>().CurrentState.FinishState();
            _isDone = true;
        });
    }

    private bool IsTargetCorrect()
    {
        foreach(NPCController npc in _controller.WaveTargets)
        {
            if(npc == _controller.OtherCurrentNPC) { return true; }
        }

        return false;
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
        _visualController.OnAction(NPCActions.NONE);
        _controller.SetOtherNPCReference(null);
        _controller.SetWaveType(NPCWaveType.NONE);
    }
}

using UnityEngine;
using Enums;
using StateMachine;
[CreateAssetMenu(menuName ="State/WaveToNPC")]
public class NPCWaveToNPCState : NPCBaseState
{
    private bool _canCheckForStateFinish = false;

    public override void OnEnter()
    {
        base.OnEnter();
        _visualController.OnAction(NPCActions.WAVE);
        if (_controller.WaveType == NPCWaveType.WAVER)
        {
            // wave animation
            if (_controller.OtherCurrentNPC != null) //user fprced this state
            {

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
                    else
                    {
                        FinishState();
                    }
                }
            }
        }
        else
        {
            if (_controller.OtherCurrentNPC != null)
            {
                GoToNPC();
            }
            else
            {
                FinishState();
            }
        }
        _canCheckForStateFinish = true;
    }

    private void GoToNPC()
    {
        if (_controller.OtherCurrentNPC != null)
        {
            Vector2 position = _controller.OtherCurrentNPC.GetRandomValidInteractionPoint();
            _movementController.GoToPosition(position, () =>
            {
                Vector2 direction = stateController.gameObject.transform.position - _controller.OtherCurrentNPC.transform.position;
                Vector2 direction2 = _controller.OtherCurrentNPC.transform.position - stateController.gameObject.transform.position;
                _controller.OtherCurrentNPC.gameObject.GetComponent<NPCVisualController>().DetermineCardinalDirection(direction);
                _visualController.DetermineCardinalDirection(direction2);
                _controller.OtherCurrentNPC?.GetComponent<StateController>().CurrentState.FinishState();
                _isDone = true;
            });
        }
    }

    private bool IsTargetCorrect()
    {
        if(_controller.WaveTargets.Length > 0)
        {
            foreach (NPCController npc in _controller.WaveTargets)
            {
                if (npc == _controller.OtherCurrentNPC) { return true; }
            }
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

    public override void OnExit()
    {
        _movementController.InterrumptPath();
        base.OnExit();
        _visualController.OnAction(NPCActions.NONE);
        _controller.RemoveCurrentOtherNPCReference();
        _controller.SetWaveType(NPCWaveType.NONE);
    }
}

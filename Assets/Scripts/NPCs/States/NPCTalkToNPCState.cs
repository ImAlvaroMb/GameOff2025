using UnityEngine;
using Enums;
using StateMachine;
[CreateAssetMenu(menuName ="State/TalkState")]
public class NPCTalkToNPCState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        if(_controller.TalkType == TalkType.LISTENER)
        {

        } else if(_controller.TalkType == TalkType.TALKER || _controller.TalkType == TalkType.NONE)
        {
            if(_controller.OtherCurrentNPC != null) // user forced this state
            {
                GoToNPC();
            } else
            {
                _controller.SetOtherNPCReference(_awarnessController.GetNPC());
                if(_controller.OtherCurrentNPC == null)
                {
                    FinishState();
                    
                }
                _controller.OtherCurrentNPC?.SetTalkType(TalkType.LISTENER);
                GoToNPC();
            }
        }
    }

    private void GoToNPC()
    {
        if(_controller.OtherCurrentNPC != null)
        {
            _movementController.GoToPosition(_controller.OtherCurrentNPC.transform.position, () =>
            {
                _visualController.ActivateSpeechBubble(() =>
                {
                    _controller.OtherCurrentNPC.GetComponent<StateController>().CurrentState.FinishState();
                    _isDone = true;
                });
            });
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller.SetCurrentAction(NPCActions.NONE);
        _controller.RemoveCurrentOtherNPCReference();
        _controller.SetTalkType(TalkType.NONE);
    }
}

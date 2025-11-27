using UnityEngine;
using Enums;
using StateMachine;
using Unity.VisualScripting;
[CreateAssetMenu(menuName ="State/TalkState")]
public class NPCTalkToNPCState : NPCBaseState
{
    private bool _canCheckForStateFinish = false;

    public override void OnEnter()
    {
        base.OnEnter();
        _visualController.OnAction(NPCActions.TALK_TO_NPC);
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

        _canCheckForStateFinish = true;
    }

    private void GoToNPC()
    {
        if(_controller.OtherCurrentNPC != null)
        {
            Vector2 position = _controller.OtherCurrentNPC.GetRandomValidInteractionPoint();
            _movementController.GoToPosition(position, () =>
            {
                Vector2 direction = stateController.gameObject.transform.position - _controller.OtherCurrentNPC.transform.position;
                _controller.OtherCurrentNPC.gameObject.GetComponent<NPCVisualController>().DetermineCardinalDirection(direction);
                _visualController.ActivateSpeechBubble(() =>
                {
                    Vector2 direction = _controller.OtherCurrentNPC.transform.position - stateController.gameObject.transform.position;
                    _visualController.DetermineCardinalDirection(direction);
                    _isDone = true;
                    _controller.OtherCurrentNPC?.GetComponent<StateController>().CurrentState.FinishState();
                });
            });
        }
    }

    public override void OnExit()
    {
        _movementController.InterrumptPath();
        base.OnExit();
        _controller.SetCurrentAction(NPCActions.NONE);
        _controller.RemoveCurrentOtherNPCReference();
        _controller.SetTalkType(TalkType.NONE);
    }
}

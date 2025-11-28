using UnityEngine;
using Enums;
[CreateAssetMenu(menuName ="State/InteractWithObjState")]
public class NPCDoObjectInteractionState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _visualController.OnAction(NPCActions.DO_OBJECT_INTERACTION);
        if(_controller.CurrentInteractable != null) //we already have a object to interact with (means npc is being controller or disrupted)
        {
            GoToInteractable();
        } else
        {
            _controller.SetCurrentInteractable(_awarnessController.GetObjToInteractWith(_controller.GetPosition()));

            if (_controller.CurrentInteractable != null)
            {
                GoToInteractable();
            } else
            {
                FinishState();
            }

        }
    }


    private void GoToInteractable()
    {
        Vector2 position = _controller.CurrentInteractable.GetRandomValidInteractionPoint();
        _movementController.GoToPosition(position, () =>
        {
            if(_awarnessController.CanInteract(_controller.CurrentInteractable))
            {
                _controller.CurrentInteractable.Interact(_controller);
                _controller.RemoveCurrentInteractable();
            } else
            {
                AlertSystemController.Instance.SendAlert($"NPC {_controller.gameObject.name} cant interact with this interactablew", 2f);
            }
            FinishState();
        });
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller.RemoveCurrentInteractable();
        _movementController.InterrumptPath();
    }


}

using UnityEngine;
[CreateAssetMenu(menuName ="State/InteractWithObjState")]
public class NPCDoObjectInteractionState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();

        if(_controller.CurrentInteractable != null) //we already have a object to interact with (means npc is being controller or disrupted)
        {
            _isDone = true;
        } else
        {
            _controller.SetCurrentInteractable(_awarnessController.GetObjToInteractWith(_controller.GetPosition()));

            if (_controller.CurrentInteractable != null) GoToInteractable();
        }
    }


    private void GoToInteractable()
    {
        _movementController.GoToPosition(_controller.CurrentInteractable.transform.position, () =>
        {
            _controller.CurrentInteractable.Interact();
            _controller.RemoveCurrentInteractable();
            FinishState();
        });
    }


}

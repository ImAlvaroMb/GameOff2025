using UnityEngine;
using StateMachine;
using Enums;
public class NPCBaseState : State
{
    protected private NPCController _controller;
    protected private NPCMovementController _movementController;
    protected private NPCAwarness _awarnessController;

    public override void OnEnter()
    {
        _controller = stateController.gameObject.GetComponent<NPCController>();
        _movementController = stateController.gameObject.GetComponent<NPCMovementController>();
        _awarnessController = stateController.gameObject.GetComponent<NPCAwarness>();
    }
    public override void OnExit()
    {
        
    }
    public override void FinishState()
    {
        _controller.ResetAction();
        _isDone = true;
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}

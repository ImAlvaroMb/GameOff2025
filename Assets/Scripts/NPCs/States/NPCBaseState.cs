using UnityEngine;
using StateMachine;
using Enums;
public class NPCBaseState : State
{
    protected private NPCController _controller;
    protected private NPCMovementController _movementController;

    protected private NPCActions _currentAction;

    public override void OnEnter()
    {
        _controller = stateController.GetComponent<NPCController>();
        _movementController = stateController.GetComponent<NPCMovementController>();
    }
    public override void OnExit()
    {
        
    }
    public override void FinishState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}

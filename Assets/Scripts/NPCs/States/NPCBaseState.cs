using UnityEngine;
using StateMachine;
using Enums;
public class NPCBaseState : State
{
    protected private NPCController _controller;
    protected private NPCMovementController _movementController;


    public override void OnEnter()
    {
        _controller = stateController.gameObject.GetComponent<NPCController>();
        _movementController = stateController.gameObject.GetComponent<NPCMovementController>();
    }
    public override void OnExit()
    {
        
    }
    public override void FinishState()
    {
        _isDone = true;
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}

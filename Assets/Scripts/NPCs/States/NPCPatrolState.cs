using UnityEngine;

[CreateAssetMenu(menuName ="State/PatrolState")]
public class NPCPatrolState : NPCBaseState
{
    private Vector2 point;

    public override void OnEnter()
    {
        base.OnEnter();
        if(_movementController.TargetPoint == Vector2.zero)
        {
            point = _movementController.GenerateNewPointInRange(_controller.OriginalPosition, _controller.RangeToPatrol);
            _movementController.GoToPosition(point, () => FinishState());
        } else
        {
            _movementController.GoToPosition(_movementController.TargetPoint, () => FinishState());
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void OnExit()
    {
        base.OnExit();
        _movementController.InterrumptPath();
        _movementController.SetTargetPoint(Vector2.zero);
    }
}

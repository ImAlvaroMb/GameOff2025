using UnityEngine;

[CreateAssetMenu(menuName ="State/PatrolState")]
public class NPCPatrolState : NPCBaseState
{
    private Vector2 point;
    public override void OnEnter()
    {
        base.OnEnter();
        if(_controller.FollowsPath)
        {
            _movementController.StartPatrol();
            return;
        }

        if(_movementController.TargetPoint != Vector2.zero)
        {
            point = _movementController.TargetPoint;
            _movementController.GoToPosition(_movementController.TargetPoint, () => FinishState());
        } else
        {
            point = _movementController.GenerateNewPointInRange(_controller.OriginalPosition, _controller.RangeToPatrol);
            _movementController.SetTargetPoint(point);
            _movementController.GoToPosition(point, () => FinishState());
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
        CheckForPathChange();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public void CheckForPathChange()
    {
        if (point != _movementController.TargetPoint)
        {
            _movementController.InterrumptPath();
            _movementController.GoToPosition(_movementController.TargetPoint, () => FinishState());
            point = _movementController.TargetPoint;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        if(_controller.FollowsPath)
        {
            _movementController.StopPatrol();
        }
        _movementController.InterrumptPath();
        _movementController.SetTargetPoint(Vector2.zero);
    }
}

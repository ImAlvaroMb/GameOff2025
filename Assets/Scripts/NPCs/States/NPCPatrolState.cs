using UnityEngine;

[CreateAssetMenu(menuName ="State/PatrolState")]
public class NPCPatrolState : NPCBaseState
{
    private Vector2 point;

    public override void OnEnter()
    {
        base.OnEnter();
        point = _movementController.GenerateNewPointInRange(_controller.OriginalPosition, _controller.RangeToPatrol);
        _movementController.GoToPosition(point, () => FinishState());
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void OnExit()
    {
        base.OnExit();
        _movementController.InterrumptPath();
    }
}

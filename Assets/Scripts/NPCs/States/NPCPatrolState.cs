using UnityEngine;

[CreateAssetMenu(menuName ="State/PatrolState")]
public class NPCPatrolState : NPCBaseState
{
    private Vector2 point;

    public override void OnEnter()
    {
        base.OnEnter();
        point = _movementController.GenerateNewPointInRange(_controller.OriginalPosition, _controller.RangeToPatrol);
        _movementController.GoToPosition(point);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

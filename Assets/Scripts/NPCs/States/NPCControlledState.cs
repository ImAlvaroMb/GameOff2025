using UnityEngine;
using Enums;

[CreateAssetMenu(menuName ="State/Controlled")]
public class NPCControlledState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _controller.SetIsBeingControlled(false);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

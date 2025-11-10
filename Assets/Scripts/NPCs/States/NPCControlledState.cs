using UnityEngine;

[CreateAssetMenu(menuName ="State/Controlled")]
public class NPCControlledState : NPCBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        // han dle influence meter
    }
}

using UnityEngine;

public class AreaOfInfluenceInteractable : BaseInteractable
{
    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);
        AudioManager.Instance.PlayOneShot(Enums.SoundName.NPCRADIO);
    }

    public override void Highlight()
    {
        base.Highlight();
    }

    public override void Dehighlight()
    {
        base.Dehighlight();
    }
}
    
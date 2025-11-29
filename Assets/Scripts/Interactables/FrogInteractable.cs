using UnityEngine;
using Enums;
public class FrogInteractable : BaseInteractable
{
    public GameObject FrogObject;
    public NPCController[] CarrierTargetNPC;
    //private bool _isFrogInPlance = true;

    protected override void Awake()
    {
        base.Awake();
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);

        if (!CheckIfCanCarryFrog(interactingNPC))
        {
            AlertSystemController.Instance.SendAlert("THIS NPC IS NOT WORTHY OF CARRYING THE FROG", 2.5f);
            AudioManager.Instance.PlayOneShot(SoundName.NPCCANT);
            return;
        }

        if (LevelController.Instance.GetIsFrogInPlace() && CheckIfCanCarryFrog(interactingNPC) && interactingNPC.IsFullyControlled)
        {
            FrogObject.transform.parent = interactingNPC.transform;
            FrogObject.transform.position = interactingNPC.FrogCarryPos.position;
            LevelController.Instance.FrogIsMovgin(interactingNPC);
            interactingNPC.SetIsCarryingFrog(true);
        } else if(!LevelController.Instance.GetIsFrogInPlace() && CheckIfCanCarryFrog(interactingNPC)) 
        {
            LevelController.Instance.FrogIsBackToPlace(interactingNPC);
            interactingNPC.SetIsCarryingFrog(false);
            FrogObject.transform.parent = gameObject.transform;
            FrogObject.transform.position = gameObject.transform.position;
        }

        AudioManager.Instance.PlayOneShot(SoundName.NPCFROG);
    }

    private bool CheckIfCanCarryFrog(NPCController interactingNPC)
    {
        foreach (NPCController controller in CarrierTargetNPC)
        {
            if(controller == interactingNPC) { return true; }
        }

        return false;
    }

}

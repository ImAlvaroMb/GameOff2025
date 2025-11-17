using UnityEngine;

public class FrogInteractable : BaseInteractable
{
    public GameObject FrogObject;
    public NPCController CarrierTargetNPC;
    private bool _isFrogInPlance = true;

    protected override void Awake()
    {
        base.Awake();
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);

        if (interactingNPC != CarrierTargetNPC)
        {
            AlertSystemController.Instance.SendAlert("THIS NPC IS NOT WORTHY OF CARRYING THE FROG", 2.5f);
        }

        if (_isFrogInPlance && interactingNPC == CarrierTargetNPC && interactingNPC.IsFullyControlled)
        {
            _isFrogInPlance = false;
            FrogObject.transform.parent = interactingNPC.transform;
            FrogObject.transform.position = interactingNPC.FrogCarryPos.position;
            LevelController.Instance.FrogIsMovgin();
            interactingNPC.SetIsCarryingFrog(true);
        } else if(!_isFrogInPlance && interactingNPC == CarrierTargetNPC) 
        {
            _isFrogInPlance = true;
            LevelController.Instance.FrogIsBackToPlace();
            interactingNPC.SetIsCarryingFrog(false);
            FrogObject.transform.parent = gameObject.transform;
            FrogObject.transform.position = gameObject.transform.position;
        }
    }

}

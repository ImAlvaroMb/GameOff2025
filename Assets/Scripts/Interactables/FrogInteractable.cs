using UnityEngine;

public class FrogInteractable : BaseInteractable
{
    public GameObject FrogObject;
    public NPCController CarrierTargetNPC;
    private bool _isFrogInPlance = true;



    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);

        if (interactingNPC != CarrierTargetNPC)
        {
            AlertSystemController.Instance.SendAlert("THIS NPC IS NOT WORTHY OF CARRYING THE FROG", 2.5f);
        }

        if (_isFrogInPlance && interactingNPC == CarrierTargetNPC)
        {
            _isFrogInPlance = false;
            FrogObject.transform.parent = interactingNPC.transform;
            FrogObject.transform.position = interactingNPC.FrogCarryPos.position;
            LevelController.Instance.FrogIsMovgin();
        } else if(!_isFrogInPlance && interactingNPC == CarrierTargetNPC) 
        {
            _isFrogInPlance = true;
            LevelController.Instance.FrogIsBackToPlace();
            FrogObject.transform.parent = gameObject.transform;
            FrogObject.transform.position = gameObject.transform.position;
        }
    }

}

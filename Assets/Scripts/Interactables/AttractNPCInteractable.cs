using System.Collections.Generic;
using UnityEngine;
using Enums;
using StateMachine;

public class AttractNPCInteractable : BaseInteractable
{
    public List<NPCController> AttractedNPC = new List<NPCController>();

    public List<SimpleObstacleNPC> ObstacleNPC = new List<SimpleObstacleNPC>();

    [SerializeField] private bool frogHasToInteract = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);
        if((interactingNPC == null && frogHasToInteract) ||(!frogHasToInteract && interactingNPC != null))
        {
            StartVisualEffect();
            foreach (NPCController controller in AttractedNPC)
            {
                controller.SetCurrentInteractable(this);
                controller.SetCurrentAction(NPCActions.DO_OBJECT_INTERACTION);
            }

            foreach (SimpleObstacleNPC npc in ObstacleNPC)
            {
                npc.GoToPosition(GetRandomValidInteractionPoint(), () =>
                {
                    StopVisualEffect();
                    foreach (NPCController controller in AttractedNPC)
                    {
                        if (controller.CurrentInteractable == this && controller.CurrentAction == NPCActions.DO_OBJECT_INTERACTION)
                        {
                            controller.SetCurrentInteractable(null);
                            controller.GetComponent<StateController>().CurrentState.FinishState();
                        }
                    }
                    npc.GoToPosition(npc.GetInitialPos(), () => { });
                });
            }
        } else if(frogHasToInteract)
        {
            StopVisualEffect(); 
            foreach(NPCController controller in AttractedNPC)
            {
                if(controller.CurrentInteractable == this && controller.CurrentAction == NPCActions.DO_OBJECT_INTERACTION)
                {
                    controller.SetCurrentInteractable(null);
                    controller.GetComponent<StateController>().CurrentState.FinishState();
                }
            }
        } 
    }

    private void StopVisualEffect()
    {

    }

    private void StartVisualEffect()
    {
        AudioManager.Instance.PlayOneShot(SoundName.NPCRADIO);
    }


}

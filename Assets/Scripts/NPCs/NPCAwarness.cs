using UnityEngine;
using Enums;
using System.Collections.Generic;
public class NPCAwarness : MonoBehaviour
{
    private List<BaseInteractable> nearbyInteractables = new List<BaseInteractable>();
    private List<NPCController> nearbyNPC = new List<NPCController>();

    [SerializeField] private List<InteractableType> interactTypesAvailable;

    public bool CanInteract(BaseInteractable targetObject)
    {
        foreach (InteractableType type in targetObject.InteractableType)
        {
            if(interactTypesAvailable.Contains(type)) return true;
        }

        return false;
    }

    public BaseInteractable GetObjToInteractWith(Vector2 position)
    {
        if (nearbyInteractables.Count == 0) return null;
        int index = Random.Range(0, interactTypesAvailable.Count - 1);
        return nearbyInteractables[index];
    }

    public NPCController GetNPC()
    {
        if (nearbyNPC.Count == 0) return null;
        int index = Random.Range(0, nearbyNPC.Count - 1); 
        return nearbyNPC[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            BaseInteractable interactable = collision.GetComponent<BaseInteractable>();
            if (interactable != null)
            {
                if (CanInteract(interactable) && !nearbyInteractables.Contains(interactable)) nearbyInteractables.Add(interactable);
            }
        }

        if(collision.CompareTag("NPC"))
        {
            NPCController npc = collision.GetComponentInParent<NPCController>();
            if (npc != null && !nearbyNPC.Contains(npc))
            {
                nearbyNPC.Add(npc);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable"))
        {
            BaseInteractable interactable = collision.GetComponent<BaseInteractable>();
            if (interactable != null && nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Remove(interactable);
            }
        }

        if(collision.CompareTag("NPC"))
        {
            NPCController npc = collision.GetComponentInParent<NPCController>();
            if (npc != null && nearbyNPC.Contains(npc))
            {
                nearbyNPC.Remove(npc);
            }
        }
    }
}

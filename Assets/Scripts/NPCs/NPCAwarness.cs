using Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NPCAwarness : MonoBehaviour
{
    private List<BaseInteractable> nearbyInteractables = new List<BaseInteractable>();
    public List<NPCController> NearbyNPC => nearbyNPC;
    private List<NPCController> nearbyNPC = new List<NPCController>();
    private NPCController _controller;
    [SerializeField] private float newDecayDuration = 3f;
    [SerializeField] private List<InteractableType> interactTypesAvailable;

    [Header("Inmunne")]
    [SerializeField] private List<NPCController> targetNPCs = new List<NPCController>();
    private HashSet<NPCController> _affectedNPCs = new HashSet<NPCController>();
    [SerializeField] private float inmunnityRadius;

    private void Start()
    {
        _controller = GetComponent<NPCController>();
        if(_controller.IsImmune)
        {
            FindAllNPC();
        }
    }

    private void Update()
    {
        if(_controller.IsImmune)
        {
            foreach (NPCController npcController in targetNPCs)
            {
                if (npcController == null) continue;

                if (Vector2.Distance(transform.position, npcController.transform.position) < inmunnityRadius)
                {
                    if (_affectedNPCs.Add(npcController) && !npcController.IsImmune)
                    {
                        npcController.OnImmuneAreaEntered(_controller, newDecayDuration);
                    }
                }
                else
                {
                    if (_affectedNPCs.Remove(npcController) && !npcController.IsImmune)
                    {
                        npcController.OnImmuneAreaExit(_controller);
                    }
                }
            }
        }
    }

    #region Regular Interactions

    public bool CanInteract(BaseInteractable targetObject)
    {
        foreach (InteractableType type in targetObject.InteractableType)
        {
            if(interactTypesAvailable.Contains(type)) return true;
        }

        return false;
    }

    public bool IsTeacher()
    {
        foreach(InteractableType type in interactTypesAvailable)
        {
            if(type == InteractableType.TEACHER)
            {
                return true;
            }
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

    public void OnControlled()
    {
        interactTypesAvailable.Add(InteractableType.CONTROLLED);
    }

    public void OnStopControlled()
    {
        if(interactTypesAvailable.Contains(InteractableType.CONTROLLED))
            interactTypesAvailable.Remove(InteractableType.CONTROLLED);
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

    private void OnTriggerExit2D(Collider2D collision) // this takes a hevy performance toll when dealing with a lot of objects, this would usually be handled by a own system
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
    #endregion

    #region Inmunnity

    [ContextMenu("FindAllNpc")]
    private void FindAllNPC()
    {
        targetNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None).ToList();
        if(targetNPCs.Contains(_controller))
        {
            targetNPCs.Remove(_controller);
        }
        CleanUpList();
    }

    private void CleanUpList()
    {
        for (int i = targetNPCs.Count - 1; i >= 0; i--)
        {
            if (targetNPCs[i].IsImmune)
            {
                targetNPCs.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, inmunnityRadius);
    }

    #endregion
}

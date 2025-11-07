using UnityEngine;
using Enums;
using System.Collections.Generic;
public class NPCAwarness : MonoBehaviour
{
    private List<BaseInteractable> nearbyInteractables = new List<BaseInteractable>();

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
        int index = Random.Range(0, interactTypesAvailable.Count - 1);
        return nearbyInteractables[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable")) return;

        BaseInteractable interactable = collision.GetComponent<BaseInteractable>();
        if(interactable != null)
        {
            if(CanInteract(interactable)) nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable")) return;
        BaseInteractable interactable = collision.GetComponent<BaseInteractable>();

        if (interactable != null && nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Remove(interactable);
        }
    }
}

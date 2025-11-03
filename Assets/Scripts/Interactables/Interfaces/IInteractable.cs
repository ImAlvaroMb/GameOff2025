using UnityEngine;

public interface IInteractable 
{
    void Highlight();
    void Dehighlight();
    void Interact();
    bool CanInteract();
}

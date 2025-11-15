public interface IInteractable 
{
    void Highlight();
    void Dehighlight();
    void Interact(NPCController interactingNPC);
    bool CanInteract();
}

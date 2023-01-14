namespace Game.Interaction
{
    public interface IInteractable
    {
        public void Interact();

        public bool CanInteract();
        public bool CanDisplayPrompt();
        
        public bool IsHighlighted { get; set; }
    }
}
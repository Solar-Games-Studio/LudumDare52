namespace Game.Interaction
{
    public interface IInteractable
    {
        public void Interact();

        public bool CanInteract();
        public void ChangeBubbleState(bool state);
    }
}
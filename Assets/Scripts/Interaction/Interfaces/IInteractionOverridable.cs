namespace Game.Interaction
{
    public interface IInteractionOverridable
    {
        public void HandleInteractionInput(IInteractable interactable);
        public void HandleHighlight(IInteractable interactable);
    }
}
using UnityEngine;

namespace Game.Interaction
{
    public interface IInteractable
    {
        public void Interact();

        public bool CanInteract();
        public bool CanDisplayPrompt();
        
        public bool IsHighlighted { get; set; }
        public Vector3 MarkerPosition { get; }
    }
}
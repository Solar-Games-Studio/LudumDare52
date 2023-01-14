using UnityEngine;
using UnityEngine.Events;

namespace Game.Interaction
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public bool IsHighlighted { get; set; }

        public UnityEvent OnInteract;

        public virtual void Interact()
        {
            OnInteract.Invoke();
        }

        public virtual bool CanInteract() => true;
        public virtual bool CanDisplayPrompt() => true;
    }
}
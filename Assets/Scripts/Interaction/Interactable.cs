using UnityEngine;
using UnityEngine.Events;

namespace Game.Interaction
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public UnityEvent OnInteract;
        public UnityEvent<bool> OnBubbleChangeState;

        public virtual void Interact()
        {
            OnInteract.Invoke();
        }
        public virtual void ChangeBubbleState(bool state)
        {
            OnBubbleChangeState.Invoke(state);
        }
        public virtual bool CanInteract() => true;
    }
}
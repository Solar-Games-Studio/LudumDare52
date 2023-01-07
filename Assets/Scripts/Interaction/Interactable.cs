using UnityEngine;
using UnityEngine.Events;

namespace Game.Interaction
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public UnityEvent OnInteract;

        public virtual void Interact()
        {
            OnInteract.Invoke();
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace Game.Interaction
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public virtual Vector3 MarkerPosition => 
            transform.position +
            markerOffset;

        public virtual bool IsHighlighted { get; set; }

        [Label("Marker")]
        [SerializeField] Vector3 markerOffset;

        [Label("Events")]
        public UnityEvent OnInteract;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + markerOffset, 0.1f);
        }

        public virtual void Interact()
        {
            OnInteract.Invoke();
        }

        public virtual bool CanInteract() => true;

        public virtual bool CanDisplayPrompt() => true;
    }
}
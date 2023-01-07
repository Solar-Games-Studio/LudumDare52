using UnityEngine;
using qASIC.Input;

namespace Game.Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        [Label("Settings")]
        [SerializeField] Transform interactionPoint;
        [SerializeField] Vector3 offset;
        [SerializeField] float interactLength = 2f;
        [SerializeField] LayerMask interactableLayer;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_interact;

        bool _didHit;
        RaycastHit _raycastHit;
        IInteractable _hitInteractable;

        private void FixedUpdate()
        {
            _didHit = Physics.Raycast(transform.position + offset, Vector3.down, out _raycastHit, interactLength, interactableLayer);
            _hitInteractable = _raycastHit.transform?.GetComponent<IInteractable>();
            _didHit = _hitInteractable != null;
        }

        private void Update()
        {
            if (_didHit && i_interact.GetInputDown())
                Interact();
        }

        public void Interact()
        {
            _hitInteractable.Interact();
        }

        private void OnDrawGizmos()
        {
            const float arrowHeadLengthMultiplier = 0.2f;

            if (interactionPoint == null)
                return;

            float arrowHeadLength = interactLength * arrowHeadLengthMultiplier;

            Gizmos.color = _didHit ? Color.green : Color.red;
            Vector3 startPos = interactionPoint.position + offset;
            Vector3 endPos = startPos + Vector3.down * interactLength;
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawLine(endPos, endPos + new Vector3(arrowHeadLength, arrowHeadLength, 0f));
            Gizmos.DrawLine(endPos, endPos + new Vector3(-arrowHeadLength, arrowHeadLength, 0f));
            Gizmos.DrawLine(endPos, endPos + new Vector3(0f, arrowHeadLength, arrowHeadLength));
            Gizmos.DrawLine(endPos, endPos + new Vector3(0f, arrowHeadLength, -arrowHeadLength));
        }
    }
}
using UnityEngine;
using qASIC.Input;
using UnityEngine.Events;
using System.Collections.Generic;
using qASIC;

namespace Game.Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        [Label("Settings")]
        [DynamicHelp(nameof(GetHelpText))]
        [SerializeField] Transform interactionPoint;
        [SerializeField] Vector3 offset;
        [SerializeField] float interactLength = 2f;
        [SerializeField] LayerMask interactableLayer;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_interact;

        [Label("Events")]
        public UnityEvent<IInteractable> e_onInteract;

        bool _didHit;
        RaycastHit _raycastHit;
        IInteractable _hitInteractable;

        List<IInteractionOverridable> _overrides = new List<IInteractionOverridable>();

        private void FixedUpdate()
        {
            _didHit = Physics.Raycast(transform.position + (transform.forward * offset.z + transform.up * offset.y + transform.right * offset.x), Vector3.down, out _raycastHit, interactLength, interactableLayer);
            _hitInteractable = _raycastHit.transform?.GetComponent<IInteractable>();
            _didHit = _hitInteractable != null;
        }

        private void Update()
        {
            if (i_interact.GetInputDown())
                Interact();
        }

        private void OnDrawGizmos()
        {
            const float arrowHeadLengthMultiplier = 0.2f;

            if (interactionPoint == null)
                return;

            float arrowHeadLength = interactLength * arrowHeadLengthMultiplier;

            Gizmos.color = _didHit ? Color.green : Color.red;
            Vector3 startPos = interactionPoint.position + (transform.forward * offset.z + transform.up * offset.y + transform.right * offset.x);
            Vector3 endPos = startPos + Vector3.down * interactLength;
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawLine(endPos, endPos + new Vector3(arrowHeadLength, arrowHeadLength, 0f));
            Gizmos.DrawLine(endPos, endPos + new Vector3(-arrowHeadLength, arrowHeadLength, 0f));
            Gizmos.DrawLine(endPos, endPos + new Vector3(0f, arrowHeadLength, arrowHeadLength));
            Gizmos.DrawLine(endPos, endPos + new Vector3(0f, arrowHeadLength, -arrowHeadLength));
        }

        string GetHelpText() =>
            $"Overrides count: {_overrides.Count}";

        public void Interact()
        {
            if (_overrides.Count != 0)
            {
                _overrides[_overrides.Count - 1].HandleInteractionInput(_hitInteractable);
                return;
            }

            if (!_didHit) return;

            _hitInteractable.Interact();
            e_onInteract.Invoke(_hitInteractable);
        }

        public void OverrideInteraction(IInteractionOverridable overridable) =>
            _overrides.Add(overridable);

        public void RemoveInteractionOverride(IInteractionOverridable overridable)
        {
            if (_overrides.Contains(overridable))
                _overrides.Remove(overridable);
        }
    }
}
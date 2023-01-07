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
        [SerializeField] int rayAmount = 3;
        [SerializeField] Vector3 startPos;
        [SerializeField] Vector3 endPos;
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
            _didHit = false;
            _raycastHit = new RaycastHit();

            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 start = interactionPoint.position + CalculateOffset(Vector3.Lerp(startPos, endPos, i / Mathf.Max(1f, rayAmount - 1)));
                if (!Physics.Raycast(start, -interactionPoint.forward, out RaycastHit hit, interactLength, interactableLayer)) continue;
                _didHit = true;
                _raycastHit = hit;
                break;
            }

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
            if (interactionPoint == null)
                return;

            Gizmos.color = _didHit ? Color.green : Color.red;

            Gizmos.DrawLine(interactionPoint.position + startPos, interactionPoint.position + endPos);

            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 start = interactionPoint.position + CalculateOffset(Vector3.Lerp(startPos, endPos, i / Mathf.Max(1f, rayAmount - 1)));
                Vector3 end = start - interactionPoint.forward * interactLength;  
                Gizmos.DrawLine(start, end);
            }
        }

        Vector3 CalculateOffset(Vector3 offset) =>
            (interactionPoint.forward * offset.z + interactionPoint.up * offset.y + interactionPoint.right * offset.x);

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
using UnityEngine;
using qASIC.Input;
using UnityEngine.Events;
using System.Collections.Generic;
using qASIC;
using Game.Prompts;

namespace Game.Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        [Label("Settings")]
        [DynamicHelp(nameof(GetHelpText))]
        [SerializeField] Transform interactionPoint;
        [SerializeField] int rayAmount = 3;

        [Space]
        [SerializeField] Vector3 additionalPos;
        [SerializeField] float additionalInteractLength;

        [Space]
        [SerializeField] Vector3 startPos;
        [SerializeField] Vector3 endPos;
        [SerializeField] float interactLength = 2f;
        [SerializeField] LayerMask interactableLayer;

        [Label("Input")]
        [EditorButton(nameof(ForceClick))]
        [SerializeField] InputMapItemReference i_interact;

        [Label("Prompts")]
        [SerializeField] Prompt interactPrompt;

        [Label("Events")]
        public UnityEvent<IInteractable> e_onInteract;

        bool _didHit;
        RaycastHit _raycastHit;
        IInteractable _hitInteractable;

        List<IInteractionOverridable> _overrides = new List<IInteractionOverridable>();

        private void FixedUpdate()
        {
            bool previousHit = _didHit;

            DetectInteractables();
            qDebug.DisplayValue("_didHitInteractable", _didHit);

            if (_didHit != previousHit)
                interactPrompt.ChangeState(_didHit);
        }

        void DetectInteractables()
        {
            _didHit = Physics.Raycast(interactionPoint.position + CalculateOffset(additionalPos), 
                -interactionPoint.forward, 
                out _raycastHit, 
                additionalInteractLength, 
                interactableLayer);

            if (CheckForInteractable(_raycastHit))
                return;

            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 start = interactionPoint.position + 
                    CalculateOffset(Vector3.Lerp(startPos, endPos, i / Mathf.Max(1f, rayAmount - 1)));

                if (!Physics.Raycast(start, 
                    -interactionPoint.up, 
                    out RaycastHit hit, 
                    interactLength, 
                    interactableLayer)) continue;

                _didHit = true;

                if (CheckForInteractable(hit))
                    return;
            }


            bool CheckForInteractable(RaycastHit hit)
            {
                qDebug.DisplayValue("_didHitObject", _didHit);

                if (!_didHit)
                    return false;

                _hitInteractable = hit.transform?.GetComponent<IInteractable>();
                _didHit = _hitInteractable != null && _hitInteractable.CanInteract();

                if (_didHit)
                    _raycastHit = hit;

                return _didHit;
            }
        }

        private void Update()
        {
            if (i_interact.GetInputDown() || _forceClick)
                Interact();

            _forceClick = false;
        }

        private void OnDrawGizmos()
        {
            if (interactionPoint == null)
                return;

            Gizmos.color = _didHit ? Color.green : Color.red;

            Gizmos.DrawLine(interactionPoint.position + CalculateOffset(startPos), interactionPoint.position + CalculateOffset(endPos));

            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 start = interactionPoint.position + 
                    CalculateOffset(Vector3.Lerp(startPos, endPos, i / Mathf.Max(1f, rayAmount - 1)));

                Vector3 end = start - interactionPoint.up * interactLength;  
                Gizmos.DrawLine(start, end);
            }

            Gizmos.color = Color.yellow;
            Vector3 additionalStart = interactionPoint.position + CalculateOffset(additionalPos);
            Gizmos.DrawLine(additionalStart, additionalStart - interactionPoint.forward * additionalInteractLength);
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

        public void OverrideInteraction(IInteractionOverridable overridable)
        {
            _overrides.Add(overridable);
        }

        public void RemoveInteractionOverride(IInteractionOverridable overridable)
        {
            if (_overrides.Contains(overridable))
                _overrides.Remove(overridable);
        }

        bool _forceClick;
        void ForceClick() =>
            _forceClick = true;
    }
}
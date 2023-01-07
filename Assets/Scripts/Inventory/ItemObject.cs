using UnityEngine;
using Game.Interaction;
using Game.Character;

namespace Game.Inventory
{
    public class ItemObject : Interactable
    {
        const string MOVEMENT_MULTIPLIER_IDENTIFIER = "held_item";

        public enum State { Free, PickedUp, Placed, }

        public State CurrentState { get; private set; } = State.Free;

        [Label("Item Data")]
        public string itemName = "item";
        public float movementMultiplier = 1f;

        [Label("Components")]
        [SerializeField] Rigidbody rb;
        [SerializeField] Collider coll;

        Transform _followTarget;

        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
        }

        private void LateUpdate()
        {
            if (_followTarget != null)
            {
                transform.position = _followTarget.position;
                transform.eulerAngles = _followTarget.eulerAngles;
            }
        }

        public void SetFollowTarget(Transform transform) =>
            _followTarget = transform;

        public virtual void ChangeState(State state)
        {
            switch (state)
            {
                case State.Free:
                    CharacterMovement.ChangeMultiplier(MOVEMENT_MULTIPLIER_IDENTIFIER, 1f);
                    if (rb != null)
                        rb.isKinematic = false;

                    if (coll != null)
                    {
                        coll.enabled = true;
                        coll.isTrigger = false;
                    }
                    break;
                case State.PickedUp:
                    CharacterMovement.ChangeMultiplier(MOVEMENT_MULTIPLIER_IDENTIFIER, movementMultiplier);
                    if (rb != null)
                        rb.isKinematic = true;

                    if (coll != null)
                    {
                        coll.enabled = false;
                        coll.isTrigger = false;
                    }
                    break;
                case State.Placed:
                    CharacterMovement.ChangeMultiplier(MOVEMENT_MULTIPLIER_IDENTIFIER, 1f);
                    if (rb != null)
                        rb.isKinematic = true;

                    if (coll != null)
                    {
                        coll.enabled = true;
                        coll.isTrigger = true;
                    }
                    break;
            }
        }

        public void Throw(Vector3 force)
        {
            ChangeState(State.Free);
            if (rb != null)
                rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
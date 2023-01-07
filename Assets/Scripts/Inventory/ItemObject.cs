using UnityEngine;
using Game.Interaction;

namespace Game.Inventory
{
    public class ItemObject : Interactable
    {
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

        public void ChangeState(State state)
        {
            switch (state)
            {
                case State.Free:
                    if (rb != null)
                        rb.isKinematic = false;

                    if (coll != null)
                    {
                        coll.enabled = true;
                        coll.isTrigger = false;
                    }
                    break;
                case State.PickedUp:
                    if (rb != null)
                        rb.isKinematic = true;

                    if (coll != null)
                    {
                        coll.enabled = false;
                        coll.isTrigger = false;
                    }
                    break;
                case State.Placed:
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
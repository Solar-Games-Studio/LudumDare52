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

        [Label("Prompts")]
        [SerializeField] Prompts.Prompt prompt_pickup;

        [Label("Layers")]
        [SerializeField] [Layer] int pickedUpLayer = 0;

        int _defaultLayer;

        Transform _followTarget;

        public override bool IsHighlighted 
        {
            get
            {
                return base.IsHighlighted;
            }
            set
            {
                _refreshPrompt = true;
                base.IsHighlighted = value;
            }
        }

        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
        }

        private void Awake()
        {
            _defaultLayer = gameObject.layer;
        }

        private void LateUpdate()
        {
            if (_followTarget != null)
            {
                transform.position = _followTarget.position;
                transform.eulerAngles = _followTarget.eulerAngles;
            }
        }

        bool _refreshPrompt = false;

        private void FixedUpdate()
        {
            if (_refreshPrompt)
            {
                prompt_pickup?.ChangeState(IsHighlighted);
                _refreshPrompt = false;
            }
        }

        public override bool CanDisplayPrompt() =>
            prompt_pickup == null;

        public void SetFollowTarget(Transform transform) =>
            _followTarget = transform;

        public virtual void ChangeState(State state)
        {
            switch (state)
            {
                case State.Free:
                    ChangeLayer(gameObject, _defaultLayer);
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
                    ChangeLayer(gameObject, pickedUpLayer);
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
                    ChangeLayer(gameObject, _defaultLayer);
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

        void ChangeLayer(GameObject g, int layer)
        {
            g.layer = layer;
            for (int i = 0; i < g.transform.childCount; i++)
                ChangeLayer(g.transform.GetChild(i).gameObject, layer);
        }

        public void Throw(Vector3 force)
        {
            ChangeState(State.Free);
            if (rb != null)
                rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
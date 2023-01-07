using UnityEngine;
using Game.Interaction;
using qASIC.Input;

namespace Game.Inventory
{
    public class CharacterInventory : MonoBehaviour, IInteractionOverridable
    {
        [Label("Throwing")]
        [SerializeField] Vector3 force;
        
        [Label("Components")]
        [SerializeField] CharacterInteraction interaction;
        [SerializeField] Transform itemHolder;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_throw;

        ItemObject _heldItem;

        private void Reset()
        {
            interaction = GetComponent<CharacterInteraction>();
        }

        private void Awake()
        {
            interaction.e_onInteract.AddListener(OnInteract);
        }

        private void Update()
        {
            if (_heldItem != null && i_throw.GetInputDown())
                Throw();
        }

        public void Throw()
        {
            var item = _heldItem;
            UnEquipItem();
            item.Throw(transform.forward * force.z + transform.up * force.y + transform.right * force.x);
        }

        public void HandleInteractionInput(IInteractable interactable)
        {
            if (interactable is IItemHolder itemHolder)
            {
                PlaceItem(itemHolder);
                return;
            }

            UnEquipItem();
        }

        public void OnInteract(IInteractable interactable)
        {
            if (!(interactable is ItemObject item))
                return;

            EquipItem(item);
        }

        void EquipItem(ItemObject item)
        {
            _heldItem = item;
            _heldItem.ChangeState(ItemObject.State.PickedUp);
            _heldItem.SetFollowTarget(itemHolder);
            interaction.OverrideInteraction(this);
        }

        void UnEquipItem()
        {
            interaction.RemoveInteractionOverride(this);
            if (_heldItem == null) return;

            _heldItem.ChangeState(ItemObject.State.Free);
            _heldItem.SetFollowTarget(null);
            _heldItem = null;
        }

        void PlaceItem(IItemHolder holder)
        {
            if (_heldItem == null) return;

            _heldItem.ChangeState(ItemObject.State.Placed);
            _heldItem.SetFollowTarget(holder.HolderTransform);
            _heldItem = null;

            interaction.RemoveInteractionOverride(this);
        }
    }
}
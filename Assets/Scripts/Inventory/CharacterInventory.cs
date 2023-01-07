using UnityEngine;
using Game.Interaction;
using qASIC.Input;
using UnityEngine.WSA;

namespace Game.Inventory
{
    public class CharacterInventory : Player.PlayerBehaviour, IInteractionOverridable
    {
        [Label("Throwing")]
        [SerializeField] Vector3 force;
        
        [Label("Components")]
        [SerializeField] CharacterInteraction interaction;
        [SerializeField] Transform itemHolder;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_throw;

        public ItemObject HeldItem { get; private set; }

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
            if (HeldItem != null && i_throw.GetInputDown())
                Throw();
        }

        public void Throw()
        {
            var item = HeldItem;
            UnEquipItem();
            item.Throw(transform.forward * force.z + transform.up * force.y + transform.right * force.x);
        }

        public void HandleInteractionInput(IInteractable interactable)
        {
            if (interactable is IItemInteractable itemInteractable && !itemInteractable.ItemInteract())
                return;

            if (interactable is IItemHolder itemHolder && itemHolder.CanHold())
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
            HeldItem = item;
            HeldItem.ChangeState(ItemObject.State.PickedUp);
            HeldItem.SetFollowTarget(itemHolder);
            interaction.OverrideInteraction(this);
        }

        void UnEquipItem()
        {
            interaction.RemoveInteractionOverride(this);
            RemoveItem();
        }

        void PlaceItem(IItemHolder holder)
        {
            if (HeldItem == null) return;

            HeldItem.ChangeState(ItemObject.State.Placed);
            HeldItem.SetFollowTarget(holder.HolderTransform);
            HeldItem = null;

            interaction.RemoveInteractionOverride(this);
        }

        public ItemObject RemoveItem()
        {
            if (HeldItem == null) return null;

            var item = HeldItem;
            HeldItem.ChangeState(ItemObject.State.Free);
            HeldItem.SetFollowTarget(null);
            HeldItem = null;
            return item;
        }
    }
}
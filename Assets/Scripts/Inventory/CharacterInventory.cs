﻿using UnityEngine;
using Game.Interaction;
using Game.Character;
using qASIC.Input;
using Game.Prompts;

namespace Game.Inventory
{
    public class CharacterInventory : Player.PlayerBehaviour, IInteractionOverridable
    {
        [Label("Throwing")]
        [SerializeField] Character.CharacterMovement movement;
        [SerializeField] float movementForceMultiplier = 6f;
        [SerializeField] Vector3 force;
        
        [Label("Components")]
        [SerializeField] CharacterInteraction interaction;
        [SerializeField] Transform itemHolder;
        [SerializeField] CharacterAnimation characterAnimation;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_throw;

        [Label("Prompts")]
        [SerializeField] Prompt prompt_drop;
        [SerializeField] Prompt prompt_throw;

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
            item.Throw(transform.forward * force.z + 
                transform.up * force.y + 
                transform.right * force.x +
                movement.Direction * movementForceMultiplier);
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

        public void EquipItem(ItemObject item)
        {
            if (HeldItem != null) return;

            characterAnimation?.SetHoldingState(true);
            HeldItem = item;
            HeldItem.ChangeState(ItemObject.State.PickedUp);
            HeldItem.SetFollowTarget(itemHolder);
            interaction.OverrideInteraction(this);

            prompt_drop.ChangeState(true);
            prompt_throw.ChangeState(true);
        }

        void UnEquipItem()
        {
            interaction.RemoveInteractionOverride(this);
            characterAnimation?.SetHoldingState(false);
            RemoveItem();
        }

        void PlaceItem(IItemHolder holder)
        {
            if (HeldItem == null) return;

            HeldItem.ChangeState(ItemObject.State.Placed);
            HeldItem.SetFollowTarget(holder.HolderTransform);
            characterAnimation?.SetHoldingState(false);
            HeldItem = null;

            prompt_drop.ChangeState(false);
            prompt_throw.ChangeState(false);

            interaction.RemoveInteractionOverride(this);
        }

        public ItemObject RemoveItem()
        {
            if (HeldItem == null) return null;

            var item = HeldItem;
            HeldItem.ChangeState(ItemObject.State.Free);
            HeldItem.SetFollowTarget(null);
            characterAnimation?.SetHoldingState(false);
            HeldItem = null;
            interaction.RemoveInteractionOverride(this);

            prompt_drop.ChangeState(false);
            prompt_throw.ChangeState(false);
            return item;
        }
    }
}
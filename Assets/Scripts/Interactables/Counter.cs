using Game.Interaction;
using Game.Inventory;
using UnityEngine;
using Game.Ordering;
using Game.Player;
using Game.Harvestables;
using Game.Prompts;

namespace Game.Interactables
{
    public class Counter : Interactable, IItemInteractable
    {
        [SerializeField] Prompt givePrompt;

        bool _updatePrompt = false;

        private void FixedUpdate()
        {
            if (_updatePrompt)
            {
                givePrompt.ChangeState(IsHighlighted);
                _updatePrompt = false;
            }
        }

        public override bool IsHighlighted 
        { 
            get => base.IsHighlighted;
            set
            {
                _updatePrompt = true;
                base.IsHighlighted = value;
            }
        }

        public bool ItemInteract()
        {
            var inventory = PlayerReference.Singleton.GetComponent<CharacterInventory>();

            if (inventory.HeldItem is PopCornObject popcorn)
            {
                OrderManager.Singleton?.FinishPreparingItem(new OrderManager.PreparationItem()
                {
                    burned = popcorn.burned,
                    materials = popcorn.materials,
                });

                Destroy(inventory.HeldItem.gameObject);
                return false;
            }

            return true;
        }

        public bool CanDisplayDropPrompt()
        {
            var inventory = PlayerReference.Singleton.GetComponent<CharacterInventory>();
            return inventory.HeldItem is not PopCornObject;
        }

        public override bool CanDisplayPrompt() =>
            false;

        public override bool CanInteract() =>
            base.CanInteract() &&
            PlayerReference.Singleton.GetBehaviour<CharacterInventory>().HeldItem is PopCornObject;
    }
}
using UnityEngine;
using Game.Harvestables;
using Game.Harvestables.Materials;
using Game.UI;
using Game.Interaction;

namespace Game.Interactables
{
    public class CornCart : Interactable, Inventory.IItemInteractable
    {
        [SerializeField] UIToggleCallback UI;
        [SerializeField] HarvestableMaterial corn;
        [SerializeField] UsableMaterial[] usableMaterials;

        public Transform HolderTransform => null;

        public bool ItemInteract()
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<Inventory.CharacterInventory>();

            if (!(inventory.HeldItem is HarvestableMaterialObject materialObject))
                return true;

            if (materialObject.material == corn)
            {
                //Open UI
                UI.OnToggle?.Invoke(this, true);
                Character.CharacterMovement.ChangeMultiplier(GetInstanceID().ToString(), 0f);
                return false;
            }

            foreach (var usableMaterial in usableMaterials)
            {
                if (usableMaterial.material != materialObject.material) continue;

                if (usableMaterial.count >= usableMaterial.limit)
                    return true;

                usableMaterial.count++;
                inventory.RemoveItem();
                Destroy(materialObject.gameObject);
                return false;
            }

            return true;
        }

        public void CloseUI()
        {
            Character.CharacterMovement.ChangeMultiplier(GetInstanceID().ToString(), 1f);
            UI.OnToggle?.Invoke(this, false);
        }

        public override void Interact()
        {          
            base.Interact();
        }

        [System.Serializable]
        public class UsableMaterial
        {
            public HarvestableMaterial material;
            public int count;
            public int limit = 2;
        }
    }
}
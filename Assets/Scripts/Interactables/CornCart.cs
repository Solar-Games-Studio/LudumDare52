using UnityEngine;
using qASIC.Toggling;
using Game.Harvestables;
using Game.Harvestables.Materials;

namespace Game.Interaction
{
    public class CornCart : Interactable, Inventory.IItemHolder
    {
        [SerializeField] string togglerName;
        [SerializeField] UsableMaterial[] usableMaterials;

        public Transform HolderTransform => null;

        public bool CanHold()
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<Inventory.CharacterInventory>();

            if (!(inventory.HeldItem is HarvestableMaterialObject materialObject))
                return false;

            foreach (var usableMaterial in usableMaterials)
            {
                if (usableMaterial.material != materialObject.material) continue;
                usableMaterial.count = Mathf.Clamp(usableMaterial.count + 1, 0, usableMaterial.limit);
                inventory.RemoveItem();
                Destroy(materialObject.gameObject);
                return false;
            }

            return false;
        }

        public override void Interact()
        {          
            StaticToggler.ChangeState(togglerName, true);
            Character.CharacterMovement.ChangeMultiplier(GetInstanceID().ToString(), 0f);
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
using UnityEngine;
using Game.Harvestables;
using Game.Harvestables.Materials;
using Game.UI;
using Game.Interaction;
using System.Collections.Generic;
using Game.Inventory;

namespace Game.Interactables
{
    public class CornCart : Interactable, IItemInteractable
    {
        [SerializeField] UIToggleCallback UI;
        [SerializeField] HarvestableMaterial corn;
        public UsableMaterial[] usableMaterials;

        [Label("Prefabs")]
        [SerializeField] ItemObject cornObject;
        [SerializeField] PopCornObject popcornObject;
        [SerializeField] PopCornObject burnedObject;

        [Label("Timing")]
        [SerializeField] float prepareTime;
        [SerializeField] float overcoockedTime;

        public Transform HolderTransform => null;

        bool _isMakingPopcorn;
        List<HarvestableMaterial> _popcornMaterials;
        float _startTime;

        public bool ItemInteract()
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            if (_isMakingPopcorn)
            {
                if (inventory.HeldItem != null)
                    return true;

                StopMakingPopcorn();

                return false;
            }

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

        public void MakePopcorn(List<int> materials)
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();
            Destroy(inventory.RemoveItem().gameObject);

            _popcornMaterials = new List<HarvestableMaterial>();
            _isMakingPopcorn = true;
            _startTime = Time.time;

            foreach (var material in materials)
            {
                usableMaterials[material].count--;
                _popcornMaterials.Add(usableMaterials[material].material);
            }
        }

        void StopMakingPopcorn()
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            _isMakingPopcorn = false;
            float t = Time.time - _startTime;
            if (t < prepareTime)
            {
                var corn = Instantiate(cornObject);
                inventory.EquipItem(corn);
                return;
            }

            if (t < prepareTime - overcoockedTime)
            {
                var popcorn = Instantiate(popcornObject);
                popcorn.materials = _popcornMaterials;
                inventory.EquipItem(popcorn);
                return;
            }

            var burned = Instantiate(burnedObject);
            burned.materials = _popcornMaterials;
            inventory.EquipItem(burned);
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
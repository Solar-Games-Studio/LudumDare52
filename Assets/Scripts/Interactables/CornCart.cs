using UnityEngine;
using Game.Harvestables;
using Game.Harvestables.Materials;
using Game.UI;
using Game.Interaction;
using System.Collections.Generic;
using Game.Inventory;
using UnityEngine.UI;
using static Game.Interactables.CornCart;

namespace Game.Interactables
{
    public class CornCart : Interactable, IItemInteractable
    {
        [Label("UI")]
        [SerializeField] UIToggleCallback UI;
        [SerializeField] Slider timerSlider;

        [Label("Materials")]
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

        private void Awake()
        {
            if (timerSlider != null)
                timerSlider.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_isMakingPopcorn) return;

            timerSlider.value = (Time.time - _startTime) / (prepareTime + overcoockedTime);
        }

        public bool ItemInteract()
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            if (inventory.HeldItem is not HarvestableMaterialObject materialObject)
                return true;

            if (!_isMakingPopcorn && materialObject.material == corn)
            {
                //Open UI
                UI.OnToggle?.Invoke(this, true);
                Character.CharacterMovement.ChangeMultiplier(GetInstanceID().ToString(), 0f);
                return false;
            }

            if (CanDepositMaterial(materialObject.material, out int i))
            {
                usableMaterials[i].count++;
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
            timerSlider?.gameObject.SetActive(true);
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
            timerSlider?.gameObject.SetActive(false);
            float t = Time.time - _startTime;
            if (t < prepareTime)
            {
                var corn = Instantiate(cornObject);
                inventory.EquipItem(corn);
                return;
            }

            if (t < prepareTime + overcoockedTime)
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
            if (_isMakingPopcorn)
                StopMakingPopcorn();
        }

        public override bool CanInteract()
        {
            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            if (inventory.HeldItem is HarvestableMaterialObject materialObject)
            {
                if (CanDepositMaterial(materialObject.material))
                    return true;

                return materialObject.material == corn;
            }

            if (_isMakingPopcorn && inventory.HeldItem == null)
                return true;

            return false;
        }

        bool CanDepositMaterial(HarvestableMaterial material) =>
            CanDepositMaterial(material, out _);

        bool CanDepositMaterial(HarvestableMaterial material, out int usableMaterialIndex)
        {
            usableMaterialIndex = -1;

            for (int i = 0; i < usableMaterials.Length; i++)
            {
                var usableMaterial = usableMaterials[i];
                if (usableMaterial.material != material) continue;

                if (usableMaterial.count >= usableMaterial.limit)
                    return false;

                usableMaterialIndex = i;
                return true;
            }

            return false;
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
using Codice.CM.Common;
using Game.Harvestables.Materials;
using Game.Inventory;
using UnityEngine;
using Game.Interaction;
using System.Collections;
using Game.Character;

namespace Game.Harvestables
{
    public class Plant : Interactable, IItemInteractable
    {
        [SerializeField] GameObject emptyModel;
        [SerializeField] PlantableMaterials[] _plantableMaterials;
        [SerializeField] float harvestingTime = 1.0f;

        bool _isPlanted;
        bool _isGrowing;
        float _timePlanted;
        int _materialIndex;

        GameObject _model;

        private void Awake()
        {
            _model = emptyModel;
        }

        private void FixedUpdate()
        {
            if (!_isGrowing) return;

            var seed = _plantableMaterials[_materialIndex];
            if (Time.time - _timePlanted < seed.seed.growTime)
                return;

            ChangeModel(seed.grownModel);
            _isGrowing = false;
        }

        public bool ItemInteract()
        {
            if (_isPlanted)
                return true;

            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            if (inventory.HeldItem is not SeedObject seedObject)
                return true;

            for (int i = 0; i < _plantableMaterials.Length; i++)
            {
                if (_plantableMaterials[i].seed != seedObject.material) continue;

                _timePlanted = Time.time;
                _isPlanted = true;
                _isGrowing = true;
                _materialIndex = i;

                ChangeModel(_plantableMaterials[i].plantedModel);

                Destroy(inventory.RemoveItem().gameObject);
                return false;
            }

            return true;
        }

        public override void Interact()
        {
            base.Interact();
            if (!_isPlanted || _isGrowing) return;
            StartCoroutine(StartHarvesting());
        }

        void ChangeModel(GameObject newModel)
        {
            SetModelActive(false);
            _model = newModel;
            SetModelActive(true);
        }

        void SetModelActive(bool value)
        {
            if (_model != null)
                _model.SetActive(value);
        }
        IEnumerator StartHarvesting()
        {
            var playerReference = Player.PlayerReference.Singleton;

            _isPlanted = false;
            playerReference.GetBehaviour<CharacterAnimation>().SetHarvestingState(true);
            playerReference.GetBehaviour<CharacterMovement>().LockMovement();

            yield return new WaitForSeconds(harvestingTime);
            
            Harvest();
        }
        void Harvest()
        {
            var playerReference = Player.PlayerReference.Singleton;

            playerReference.GetBehaviour<CharacterAnimation>().SetHarvestingState(false);
            playerReference.GetBehaviour<CharacterMovement>().UnlockMovement();
            var inventory = playerReference.GetBehaviour<CharacterInventory>();

            var seed = _plantableMaterials[_materialIndex];
            var seedObject = Instantiate(seed.finalProduct);

            ChangeModel(emptyModel);
            inventory.EquipItem(seedObject);
        }

        [System.Serializable]
        public class PlantableMaterials
        {
            public HarvestableMaterial seed;
            public HarvestableMaterialObject finalProduct;
            public GameObject plantedModel;
            public GameObject grownModel;
        }
    }
}
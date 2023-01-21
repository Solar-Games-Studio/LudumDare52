using System.Linq;
using Game.Harvestables.Materials;
using Game.Inventory;
using UnityEngine;
using Game.Interaction;
using System.Collections;
using Game.Character;
using System.Collections.Generic;

namespace Game.Harvestables
{
    public class Plant : Interactable, IItemInteractable
    {
        const float HARVESTING_TIME = 0.2f;

        [SerializeField] GameObject emptyModel;
        [SerializeField] PlantableMaterials[] _plantableMaterials;
        [SerializeField] Transform bubblePosition;

        List<HarvestableMaterial> _materials = new List<HarvestableMaterial>();

        bool _isPlanted;
        bool _isGrowing;
        float _timePlanted;
        int _materialIndex;

        GameObject _model;
        Bubble bubble;

        private void Awake()
        {
            _model = emptyModel;
            _materials = _plantableMaterials
                .Select(x => x.seed)
                .ToList();
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

        public bool CanDisplayDropPrompt()
        {
            if (_isGrowing)
                return true;

            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            return !(inventory.HeldItem is SeedObject seedObject &&
                _plantableMaterials.Select(x => x.seed).Contains(seedObject.material));
        }

        public override void Interact()
        {
            base.Interact();
            if (!_isPlanted || _isGrowing) return;
            StartCoroutine(StartHarvesting());
        }

        public override bool CanInteract()
        {
            if (_isGrowing)
                return false;

            var inventory = Player.PlayerReference.Singleton.GetBehaviour<CharacterInventory>();

            if (_isPlanted && inventory.HeldItem == null)
                return true;

            if (inventory.HeldItem is not SeedObject seedObject)
                return false;

            return _materials.Contains(seedObject.material);
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
            playerReference.GetBehaviour<CharacterAnimation>().Harvest();

            yield return new WaitForSeconds(HARVESTING_TIME);
            
            Harvest();
        }
        void Harvest()
        {
            var playerReference = Player.PlayerReference.Singleton;

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
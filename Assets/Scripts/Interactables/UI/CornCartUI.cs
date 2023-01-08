using qASIC.Toggling;
using UnityEngine;
using Game.UI;
using System.Collections.Generic;

namespace Game.Interactables.UI
{
    public class CornCartUI : MonoBehaviour
    {
        [SerializeField] Toggler toggler;
        [SerializeField] UIToggleCallback callback;
        [SerializeField] Transform ingredientList;
        [SerializeField] CornCartUIIngriedient ingredientPrefab;

        CornCart _cornCart;

        Queue<CornCartUIIngriedient> _ingriedientPool = new Queue<CornCartUIIngriedient>();

        List<CornCartUIIngriedient> _spawnedIngriedients = new List<CornCartUIIngriedient>();

        private void Awake()
        {
            callback.OnToggle += OnToggle;
        }

        private void Reset()
        {
            toggler = GetComponent<Toggler>();
        }

        void OnToggle(object obj, bool state)
        {
            switch (state)
            {
                case true:
                    _cornCart = obj as CornCart;

                    foreach (var item in _spawnedIngriedients)
                        item.gameObject.SetActive(false);

                    _spawnedIngriedients.Clear();

                    foreach (var item in _cornCart.usableMaterials)
                    {
                        var ingredient = _ingriedientPool.TryDequeue(out CornCartUIIngriedient c) ? c : Instantiate(ingredientPrefab, ingredientList);
                        ingredient.gameObject.SetActive(true);

                        ingredient.image.sprite = item.material.image;
                        ingredient.text.text = $"{item.material.materialName}: {item.count}/{item.limit}";
                        ingredient.toggle.isOn = false;
                        ingredient.toggle.interactable = item.count > 0;

                        _spawnedIngriedients.Add(ingredient);
                    }

                    break;
                case false:
                    _cornCart = null;
                    break;
            }

            toggler.Toggle(state);
        }

        public void MakePopcorn()
        {
            List<int> ingriedients = new List<int>();
            for (int i = 0; i < _spawnedIngriedients.Count; i++)
                if (_spawnedIngriedients[i].toggle.isOn)
                    ingriedients.Add(i);

            _cornCart.MakePopcorn(ingriedients);
        }

        public void Close()
        {
            _cornCart.CloseUI();
        }
    }
}
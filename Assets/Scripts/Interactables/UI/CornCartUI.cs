using qASIC.Toggling;
using qASIC.Input;
using UnityEngine;
using Game.UI;
using System.Collections.Generic;
using Game.Prompts.UI;
using Game.Prompts;
using UnityEngine.UI;

namespace Game.Interactables.UI
{
    public class CornCartUI : MonoBehaviour
    {
        [SerializeField] Toggler toggler;
        [SerializeField] UIToggleCallback callback;
        [SerializeField] RectTransform ingredientList;
        [SerializeField] CornCartUIIngriedient ingredientPrefab;
        [SerializeField] RectTransform button;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_makePopcorn;
        [SerializeField] InputMapItemReference i_close;
        [SerializeField] InputMapItemReference[] i_ingredientShortcuts;

        [Label("Prompts")]
        [SerializeField] Prompt p_close;

        CornCart _cornCart;

        Queue<CornCartUIIngriedient> _ingriedientPool = new Queue<CornCartUIIngriedient>();

        List<CornCartUIIngriedient> _spawnedIngriedients = new List<CornCartUIIngriedient>();

        bool _ignoreFrameInput;
        bool _buttonFix;

        private void Awake()
        {
            callback.OnToggle += Callback_OnToggle;
            toggler.OnToggle += Toggler_OnToggle;
        }

        private void Update()
        {
            if (_buttonFix)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(button);
                _buttonFix = false;
            }

            if (_ignoreFrameInput)
            {
                _ignoreFrameInput = false;
                return;
            }

            if (toggler.State)
            {
                if (i_makePopcorn.GetInputDown())
                    MakePopcorn();

                if (i_close.GetInputDown())
                    Close();

                var inputLength = Mathf.Min(i_ingredientShortcuts.Length, _spawnedIngriedients.Count);
                for (int i = 0; i < inputLength; i++)
                {
                    var toggle = _spawnedIngriedients[i].toggle;
                    if (i_ingredientShortcuts[i].GetInputDown() && toggle.interactable)
                        toggle.isOn = !toggle.isOn;
                }
            }
        }

        private void Reset()
        {
            toggler = GetComponent<Toggler>();
        }

        void Callback_OnToggle(object obj, bool state)
        {
            p_close.ChangeState(state);

            switch (state)
            {
                case true:
                    _ignoreFrameInput = true;

                    _cornCart = obj as CornCart;

                    foreach (var item in _spawnedIngriedients)
                        item.gameObject.SetActive(false);

                    _spawnedIngriedients.Clear();

                    for (int i = 0; i < _cornCart.usableMaterials.Length; i++)
                    {
                        var item = _cornCart.usableMaterials[i];

                        var ingredient = _ingriedientPool.TryDequeue(out CornCartUIIngriedient c) ? c : Instantiate(ingredientPrefab, ingredientList);
                        ingredient.gameObject.SetActive(true);

                        ingredient.image.sprite = item.material.image;
                        ingredient.nameText.text = item.material.materialName;
                        ingredient.countText.text = $"{item.count}/{item.limit}";
                        ingredient.toggle.isOn = false;
                        ingredient.toggle.interactable = item.count > 0;

                        ingredient.prompt.gameObject.SetActive(i < i_ingredientShortcuts.Length);
                        ingredient.prompt.ChangeInputMapItemTarget(i_ingredientShortcuts[i]);

                        _spawnedIngriedients.Add(ingredient);
                    }

                    _buttonFix = true;
                    break;
                case false:
                    _cornCart = null;
                    break;
            }

            toggler.Toggle(state);
        }

        void Toggler_OnToggle(bool state)
        {
            PromptBar.MinOverrideLayer = state ? 1 : 0;
            PromptBar.Refresh();
        }

        private void OnDestroy()
        {
            PromptBar.MinOverrideLayer = 0;
            PromptBar.Refresh();
        }

        public void MakePopcorn()
        {
            List<int> ingriedients = new List<int>();
            for (int i = 0; i < _spawnedIngriedients.Count; i++)
                if (_spawnedIngriedients[i].toggle.isOn)
                    ingriedients.Add(i);

            _cornCart.MakePopcorn(ingriedients);
            Close();
        }

        public void Close()
        {
            _cornCart.CloseUI();
        }
    }
}
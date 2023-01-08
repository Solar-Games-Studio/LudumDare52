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

        CornCart _cornCart;

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
            toggler.Toggle(state);
            switch (state)
            {
                case true:
                    _cornCart = obj as CornCart;
                    break;
                case false:
                    _cornCart = null;
                    break;
            }
        }

        public void MakePopcorn()
        {
            _cornCart.MakePopcorn(new List<int>());
        }

        public void Close()
        {
            _cornCart.CloseUI();
        }
    }
}
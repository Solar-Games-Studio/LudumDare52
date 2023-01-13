using qASIC.Input;
using qASIC.Input.Map;
using qASIC.Input.Devices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Game.Input;

namespace Game.Prompts
{
    public class PromptDisplay : MonoBehaviour
    {
        [Label("Displaying")]
        public Image image;

        [Label("Geting image")]
        public PromptLibrary promptLibrary;
        public bool axisSprite;

        [Space]
        [EditorButton(nameof(RefreshTarget))]
        [SerializeField] InputMapItemReference item;

        InputBinding _binding;
        int _index;

        public void ChangeInputMapItemTarget(InputMapItemReference newItem)
        {
            item = newItem;
            RefreshTarget();
        }

        private void Reset()
        {
            image = GetComponent<Image>();
        }

        private void Awake()
        {
            RefreshTarget();
        }

        private void FixedUpdate()
        {
            ForceRefresh();
        }

        public void ForceRefresh()
        {
            if (image == null || _binding == null) return;

            CheckIndex();

            image.sprite = axisSprite ? 
                promptLibrary.GetSprite(_binding.keys[_index]) : 
                promptLibrary.GetSprite(_binding.keys[_index]);
        }

        void CheckIndex()
        {
            string deviceTypeName = GameInput.LastDevice.GetType().Name.Split('.').Last();
            string keyPath = promptLibrary.GetKeyPath(deviceTypeName);

            _index = 0;
            for (int i = 0; i < _binding.keys.Count; i++)
            {
                string s = _binding.keys[i].Split('/').First();
                if (s.ToLower() != keyPath.ToLower()) continue;
                _index = i;
                return;
            }
        }

        void RefreshTarget()
        {
            _binding = item.GetItem() as InputBinding;
        }
    }
}
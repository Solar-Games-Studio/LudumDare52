using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Pool;
using System;
using System.Linq;

namespace Game.Prompts.UI
{
    public class PromptBar : MonoBehaviour
    {
        [SerializeField] RectTransform parent;
        [SerializeField] PromptBarItem promptPrefab;
        [EditorButton(nameof(RefreshPrompts))]
        [EditorButton(nameof(RebuildVisibleStates))]
        [SerializeField] Prompt[] prompts;

        ObjectPool<PromptBarItem> _prefabPool;

        List<PromptBarItem> _items = new List<PromptBarItem>();
        List<bool> _states = new List<bool>();
        List<bool> _visibleStates = new List<bool>();

        bool _refreshNextFrame;

        static event Action OnRefresh;

        public static int MinOverrideLayer { get; set; } = 0;

        private void Awake()
        {
            _prefabPool = new ObjectPool<PromptBarItem>(CreateItem, TakeItemFromPool, ReturnItemToPool, DestroyItem);
            OnRefresh += () =>
            {
                RebuildVisibleStates();
                RefreshPrompts();
            };

            for (int i = 0; i < prompts.Length; i++)
            {
                _states.Add(false);
                _visibleStates.Add(false);

                var prompt = prompts[i];
                prompts[i].OnChangeState += (state) => ChangePromptState(prompt, state);
            }
        }

        private void Update()
        {
            if (_refreshNextFrame)
            {
                RefreshPrompts();
                _refreshNextFrame = false;
            }
        }

        public static void Refresh()
        {
            OnRefresh?.Invoke();
        }

        private void ChangePromptState(Prompt prompt, bool state)
        {
            int index = Array.IndexOf(prompts, prompt);
            if (index == -1) return;

            bool isVisible = _states[index];
            if (isVisible == state) return;
            _states[index] = state;

            _refreshNextFrame = true;
            RebuildVisibleStates();
        }

        void RebuildVisibleStates()
        {
            int overrideLayer = MinOverrideLayer;

            for (int i = 0; i < prompts.Length; i++)
            {
                if (!_states[i]) continue;
                if (prompts[i].overrideLayer > overrideLayer)
                    overrideLayer = prompts[i].overrideLayer;
            }

            for (int i = 0; i < prompts.Length; i++)
                _visibleStates[i] = _states[i] && prompts[i].overrideLayer >= overrideLayer;

            var newItemCount = _visibleStates.Where(x => x).Count();
            int difference = Mathf.Abs(_visibleStates.Where(x => x).Count() - _items.Count);

            if (difference == 0)
                return;

            switch (newItemCount > _items.Count)
            {
                case true:
                    for (int i = 0; i < difference; i++)
                    {
                        var item = _prefabPool.Get();
                        _items.Add(item);
                    }

                    break;
                case false:
                    for (int i = 0; i < difference; i++)
                    {
                        int lastItemIndex = _items.Count - 1;

                        var deleteItem = _items[lastItemIndex];
                        _items.RemoveAt(lastItemIndex);

                        _prefabPool.Release(deleteItem);
                    }

                    break;
            }
        }

        public void RefreshPrompts()
        {
            int itemIndex = 0;
            for (int i = 0; i < prompts.Length; i++)
            {
                if (!_visibleStates[i]) continue;

                var item = _items[itemIndex];
                var prompt = prompts[i];

                item.display.axisSprite = prompt.axisSprite;
                item.display.ChangeInputMapItemTarget(prompt.item);
                item.display.ForceRefresh();
                item.text.text = prompt.text;
                itemIndex++;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }

        PromptBarItem CreateItem()
        {
            var item = Instantiate(promptPrefab, parent);
            return item;
        }

        void ReturnItemToPool(PromptBarItem item) =>
            item.gameObject.SetActive(false);

        void TakeItemFromPool(PromptBarItem item)
        {
            item.gameObject.SetActive(true);
            item.transform.SetSiblingIndex(0);
        }

        void DestroyItem(PromptBarItem item) =>
            Destroy(item);
    }
}
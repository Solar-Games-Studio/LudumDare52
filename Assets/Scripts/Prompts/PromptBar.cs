using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Pool;
using System;

namespace Game.Prompts
{
    public class PromptBar : MonoBehaviour
    {
        [SerializeField] RectTransform parent;
        [SerializeField] PromptBarItem promptPrefab;
        [EditorButton(nameof(RefreshPrompts))]
        [SerializeField] Prompt[] prompts;

        ObjectPool<PromptBarItem> _prefabPool;

        List<PromptBarItem> _items = new List<PromptBarItem>();
        List<bool> _states = new List<bool>();

        bool _refreshNextFrame;

        private void Awake()
        {
            _prefabPool = new ObjectPool<PromptBarItem>(CreateItem, TakeItemFromPool, ReturnItemToPool, DestroyItem);

            for (int i = 0; i < prompts.Length; i++)
            {
                _states.Add(false);

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

        private void ChangePromptState(Prompt prompt, bool state)
        {
            int index = Array.IndexOf(prompts, prompt);
            if (index == -1) return;

            bool isVisible = _states[index];
            if (isVisible == state) return;
            _states[index] = state;

            switch (state)
            {
                case true:
                    var item = _prefabPool.Get();
                    _items.Add(item);
                    break;
                case false:
                    int lastItemIndex = _items.Count - 1;

                    var deleteItem = _items[lastItemIndex];
                    _items.RemoveAt(lastItemIndex);

                    _prefabPool.Release(deleteItem);
                    break;
            }

            _refreshNextFrame = true;
        }

        public void RefreshPrompts()
        {
            int itemIndex = 0;
            for (int i = 0; i < prompts.Length; i++)
            {
                if (!_states[i]) continue;

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
            var item = Instantiate(promptPrefab);
            item.transform.SetParent(parent);
            item.transform.SetSiblingIndex(0);
            return item;
        }

        void ReturnItemToPool(PromptBarItem item) =>
            item.gameObject.SetActive(false);

        void TakeItemFromPool(PromptBarItem item) =>
            item.gameObject.SetActive(true);

        void DestroyItem(PromptBarItem item) =>
            Destroy(item);
    }
}
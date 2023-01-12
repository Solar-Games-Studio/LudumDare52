using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game.Prompts
{
    public class PromptBar : MonoBehaviour
    {
        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var item = child.GetComponent<PromptBarItem>();
                if (item == null) continue;
                item.OnChangeState.AddListener(_ => Refresh());
            }
        }

        void Refresh()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}
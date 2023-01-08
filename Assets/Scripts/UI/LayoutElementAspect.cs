using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>Quick script for fixing Aspect Ratio Fitter not working with Layout Element</summary>
    [ExecuteInEditMode]
    public class LayoutElementAspect : MonoBehaviour
    {
        [SerializeField] LayoutElement layoutElement;

        RectTransform rectTransform;

        private void Reset()
        {
            layoutElement = GetComponent<LayoutElement>();
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            layoutElement.preferredWidth = layoutElement.preferredWidth == -1 ? -1 : rectTransform.sizeDelta.x;
            layoutElement.preferredHeight = layoutElement.preferredHeight == -1 ? -1 : rectTransform.sizeDelta.y;
        }
    }
}
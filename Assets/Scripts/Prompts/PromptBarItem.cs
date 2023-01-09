using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Prompts
{
    public class PromptBarItem : MonoBehaviour
    {
        [Label("Assign")]
        [SerializeField] Prompt prompt;
        [SerializeField] GameObject toggleObject;
        [SerializeField] RectTransform rect;
        [SerializeField] PromptDisplay display;
        [SerializeField] TMP_Text text;

        [Label("Settings")]
        [SerializeField] float width;

        [Label("Events")]
        public UnityEvent<bool> OnChangeState;

        bool _state = true;

        private void Start()
        {
            ChangeState(false);
            prompt.OnChangeState += ChangeState;
            text.text = prompt.text;
            display.axisSprite = prompt.axisSprite;
            display.ChangeInputMapItemTarget(prompt.item);
        }
        private void ChangeState(bool show)
        {
            if (show == _state) return;

            _state = show;

            rect.sizeDelta = new Vector2(_state ? width : 0f, rect.sizeDelta.y);
            (toggleObject.transform as RectTransform).localPosition = Vector3.zero;
            toggleObject.SetActive(_state);
            OnChangeState.Invoke(show);
        }
    }
}
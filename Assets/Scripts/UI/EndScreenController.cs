using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using DG.Tweening;
using qASIC.Input;
using UnityEngine.Events;

namespace Game.UI
{
    public class EndScreenController : MonoBehaviour
    {
        [SerializeField] TMP_Text header;
        [SerializeField] TMP_Text description;
        [SerializeField] CanvasGroup prompt;
        [SerializeField] Volume volume;

        [Label("Time")]
        [SerializeField] float blurDuration;
        [SerializeField] float headerDuration;
        [SerializeField] float headerDelay;
        [SerializeField] float descriptionDuration;
        [SerializeField] float descriptionDelay;
        [SerializeField] float promptDuration;
        [SerializeField] float promptDelay;
        [SerializeField] float inputDelay;

        [Label("Animation")]
        [SerializeField] Vector3 fadeInStartOffset;

        [Label("Input")]
        [SerializeField] InputMapItemReference i_continue;

        [Label("Events")]
        [EditorButton(nameof(Show))]
        [EditorButton(nameof(Hide))]
        [SerializeField] UnityEvent OnContinue;

        bool _isShown;
        float _showTime;

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (_isShown && Time.time - _showTime >= inputDelay)
            {
                if (i_continue.GetInputDown())
                    OnContinue.Invoke();
            }
        }

        public void Show()
        {
            _isShown = true;
            _showTime = Time.time;
            DOTween.To(() => volume.weight,
                x => volume.weight = x,
                1f,
                blurDuration);
            
            FadeIn(header, headerDuration, headerDelay);
            FadeIn(description, descriptionDuration, descriptionDelay);

            DOTween.Sequence()
                .AppendInterval(promptDelay)
                .Append(DOTween.To(() => prompt.alpha,
                    x => prompt.alpha = x,
                    1f,
                    promptDuration));
        }

        void FadeIn(TMP_Text text, float duration, float delay)
        {
            DOTween.Sequence()
            .AppendInterval(delay)
            .Append(DOTween.To(() => text.alpha,
                x => text.alpha = x,
                1f,
                duration));

            var pos = text.rectTransform.localPosition;
            text.rectTransform.localPosition += fadeInStartOffset;

            DOTween.Sequence()
                .AppendInterval(delay)
                .Append(DOTween.To(() => text.rectTransform.localPosition,
                    x => text.rectTransform.localPosition = x,
                    pos,
                    duration));
        }

        public void Hide()
        {
            _isShown = false;
            volume.weight = 0f;
            header.alpha = 0f;
            description.alpha = 0f;
            prompt.alpha = 0f;
        }
    }
}
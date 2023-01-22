using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using qASIC.Input;
using DG.Tweening;

namespace Game.Ordering
{
    public class OrderUI : MonoBehaviour
    {
        [Label("Visiblity")]
        [SerializeField] RectTransform iconRect;
        [SerializeField] RectTransform windowRect;
        [SerializeField] float transitionDuration = 0.5f;
        [Space]
        [SerializeField] InputMapItemReference i_toggle;

        [Label("Notification")]
        [SerializeField] CanvasGroup notification;
        [SerializeField] float notificationAppearDuration;
        [SerializeField] float notificationHideDuration;
        [SerializeField] Vector2 notificationFlyInPositionOffset = new Vector2(0f, -20f);

        [Label("Orders")]
        [SerializeField] float showCompletedDuration = 7f;
        [SerializeField] TMP_Text text;
        [SerializeField] TMP_Text noOrdersText;

        [Help("0 - order")]
        [Space]
        [TextArea]
        [SerializeField] string textFormat = "Orders:\n{0}";

        [SerializeField] string finishedIngredientFormat = "<s>{0}</s>";

        [Help("0 - item amount\n" +
            "1 - total item amount")]
        [Space]
        [SerializeField] string orderStartFormat = "- ({0}/{1}) Popcorn";

        [Space]
        [Help("0 - ingredient name")]
        [SerializeField] string orderIngriedientFormat = "<b>{0}</b>";
        [SerializeField] string orderIngriedientStart = " with ";
        [SerializeField] string orderIngriedientSeparator = " and ";

        string _text;
        bool _visible;
        float _windowXPosition;
        float _iconYPosition;

        bool _notificationVisible;

        bool _showCompleted;
        float _completeTime;

        private void Awake()
        {
            _windowXPosition = windowRect.anchoredPosition.x;
            _iconYPosition = iconRect.anchoredPosition.x;
            notification.alpha = 0f;

            windowRect.anchoredPosition = new Vector2(-_windowXPosition, windowRect.anchoredPosition.y);
        }

        private void Start()
        {
            OrderManager.Singleton.OnFinishPreparingItem += OrderManager_OnFinishPreparingItem;
            OrderManager.Singleton.OnNextOrder += OrderManager_OnNextOrder;
            OrderManager.Singleton.OnFinishOrder += OrderManager_FinishOrder;

            RebuildText(null, new List<OrderManager.PreparationItem>());
        }

        void OrderManager_OnNextOrder(Order order)
        {
            RebuildText(order, OrderManager.Singleton.Preparation);

            if (!_visible && !_notificationVisible)
            {
                _notificationVisible = true;
                var rect = notification.transform as RectTransform;
                var pos = rect.anchoredPosition;

                rect.anchoredPosition += notificationFlyInPositionOffset;
                DOTween.To(() => rect.anchoredPosition,
                    x => rect.anchoredPosition = x,
                    pos,
                    notificationAppearDuration);

                DOTween.To(() => notification.alpha,
                    x => notification.alpha = x,
                    1f,
                    notificationAppearDuration);
            }
        }

        void OrderManager_OnFinishPreparingItem(OrderManager.PreparationItem item)
        {
            RebuildText(OrderManager.Singleton.CurrentOrder, OrderManager.Singleton.Preparation);
        }

        void OrderManager_FinishOrder(Order order, OrderManager.OrderState state)
        {
            _showCompleted = true;
            _completeTime = Time.time;
            RebuildText(order, new List<OrderManager.PreparationItem>(), true);
            HideNotification();
        }

        private void Update()
        {
            var manager = OrderManager.Singleton;
            if (manager == null)
            {
                text.text = "No manager present!";
                return;
            }

            if (_showCompleted && Time.time - _completeTime >= showCompletedDuration)
            {
                RebuildText(null, new List<OrderManager.PreparationItem>());
                _showCompleted = false;
            }

            if (i_toggle.GetInputDown())
                ToggleVisibility();
        }

        public void RebuildText(Order order, List<OrderManager.PreparationItem> preparation, bool completed = false)
        {
            text.gameObject.SetActive(order != null);
            noOrdersText.gameObject.SetActive(order == null);

            if (order == null)
            {
                _text = string.Empty;
                text.text = _text;
                return;
            }

            var items = order.popcorns
                .Select(x =>
                {
                    string[] ingriedients = x.materials
                        .Select(y => string.Format(orderIngriedientFormat, y.materialName))
                        .ToArray();

                    string ingriedientsText = ingriedients.Length == 0 ?
                        string.Empty :
                        $"{orderIngriedientStart}{string.Join(orderIngriedientSeparator, ingriedients)}";

                    var preparationTemplate = new OrderManager.PreparationItem(x);

                    int preparationCount = x.amount - preparation
                        .Where(x => x.IsEqual(preparationTemplate))
                        .Count();

                    var text = $"{string.Format(orderStartFormat, preparationCount, x.amount)}{ingriedientsText}";

                    if (preparationCount >= x.amount || completed)
                        text = string.Format(finishedIngredientFormat, text);

                    return text;
                })
                .ToArray();

            _text = string.Join('\n', items);
            _text = string.Format(textFormat, _text);

            text.text = _text;
        }

        public void ToggleVisibility() =>
            ToggleVisibility(!_visible);

        public void ToggleVisibility(bool state)
        {
            if (_visible == state)
                return;

            _visible = state;

            DOTween.To(() => windowRect.anchoredPosition.x,
                x => windowRect.anchoredPosition = new Vector2(x, windowRect.anchoredPosition.y),
                _windowXPosition * (state ? 1f : -1f),
                transitionDuration);

            DOTween.To(() => iconRect.anchoredPosition.x,
                y => iconRect.anchoredPosition = new Vector2(y, iconRect.anchoredPosition.y),
                _iconYPosition * (state ? -1 : 1f),
                transitionDuration);

            if (_notificationVisible)
                HideNotification();
        }

        void HideNotification()
        {
            _notificationVisible = false;
            DOTween.To(() => notification.alpha,
                x => notification.alpha = x,
                0f,
                notificationHideDuration);
        }
    }
}
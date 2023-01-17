using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Game.Ordering
{
    public class OrderUI : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        [Help("0 - order")]
        [Space]
        [TextArea]
        [SerializeField] string textFormat = "Orders:\n{0}";

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

        private void Start()
        {
            OrderManager.Singleton.OnFinishPreparingItem += OrderManager_OnFinishPreparingItem;
            OrderManager.Singleton.OnNextOrder += OrderManager_OnNextOrder;
        }

        void OrderManager_OnNextOrder(Order order)
        {
            RebuildText(order, new List<OrderManager.PreparationItem>());
        }

        void OrderManager_OnFinishPreparingItem(OrderManager.PreparationItem item)
        {
            RebuildText(OrderManager.Singleton.CurrentOrder, OrderManager.Singleton.Preparation);
        }

        private void Update()
        {
            var manager = OrderManager.Singleton;
            if (manager == null)
            {
                text.text = "No manager present!";
                return;
            }

            text.text = _text;
        }

        public void RebuildText(Order order, List<OrderManager.PreparationItem> preparation)
        {
            if (order == null)
            {
                _text = string.Empty;
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

                    int preparationCount = preparation
                        .Where(x => x.IsEqual(preparationTemplate))
                        .Count();

                    var text = $"{string.Format(orderStartFormat, preparationCount, x.amount)}{ingriedientsText}";
                    return text;
                })
                .ToArray();

            _text = string.Join('\n', items);
            _text = string.Format(textFormat, _text);
        }
    }
}
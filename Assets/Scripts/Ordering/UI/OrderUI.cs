using TMPro;
using UnityEngine;
using System.Linq;

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

        private void Update()
        {
            var manager = OrderManager.Singleton;
            if (manager == null)
            {
                text.text = "No manager present!";
                return;
            }

            RebuildText(manager.CurrentOrder);
            text.text = _text;
        }

        public void RebuildText(Order order)
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

                    var text = $"{string.Format(orderStartFormat, 0, x.amount)}{ingriedientsText}";
                    return text;
                })
                .ToArray();

            _text = string.Join('\n', items);
            _text = string.Format(textFormat, _text);
        }
    }
}
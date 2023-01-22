using UnityEngine;
using System.Linq;

namespace Game.UI
{
    public class PopupResolutionFiller : MonoBehaviour
    {
        [SerializeField] OptionsPopup target;

        private void Reset()
        {
            target = GetComponent<OptionsPopup>();
        }

        private void Awake()
        {
            target.values = Screen.resolutions
                .Select(x => $"{x.width}x{x.height}")
                .Distinct()
                .ToArray();
        }
    }
}
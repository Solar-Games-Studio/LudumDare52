using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class VersionUI : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [SerializeField] string format = "v{0}";

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            if (text != null)
                text.text = string.Format(format, Application.version);
        }
    }
}
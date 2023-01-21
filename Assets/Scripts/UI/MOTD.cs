using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class MOTD : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [EditorButton(nameof(RandomizeMessage))]
        [SerializeField][TextArea(3, 5)] string[] messages;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            RandomizeMessage();
        }

        public void RandomizeMessage()
        {
            if (text != null && messages.Length > 0)
                text.text = messages[Random.Range(0, messages.Length)];
        }
    }
}
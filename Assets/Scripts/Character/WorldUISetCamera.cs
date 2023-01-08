using UnityEngine;

namespace Game.Character
{
    public class WorldUISetCamera : MonoBehaviour
    {
        [EditorButton(nameof(SetCamera))]
        [SerializeField] Canvas canvas;

        private void Reset()
        {
            canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            SetCamera();
        }

        void SetCamera()
        {
            canvas.worldCamera = CharacterCamera.Singleton?.cam;
        }
    }
}
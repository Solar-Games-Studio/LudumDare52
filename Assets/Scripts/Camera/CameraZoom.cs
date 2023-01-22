using qASIC.Input;
using UnityEngine;
using Game.Character;

namespace Game.Camera
{
    public class CameraZoom : MonoBehaviour
    {
        [Label("Target")]
        [SerializeField] CharacterCamera cam;

        [Label("Positions")]
        [SerializeField] Vector3 zoomMin;
        [SerializeField] Vector3 zoomMax;

        [Label("Speed")]
        [SerializeField] AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] float startZoom = 1f;
        [SerializeField] float scrollSpeed;
        [SerializeField] float buttonSpeed;

        [SerializeField] InputMapItemReference i_zoom;

        float _zoom;

        private void Reset()
        {
            cam = GetComponent<CharacterCamera>();
        }

        private void Awake()
        {
            _zoom = startZoom;
        }

        private void Update()
        {
            float move = i_zoom.GetInputValue<float>() * buttonSpeed * Time.deltaTime +
                Input.mouseScrollDelta.y * scrollSpeed;

            _zoom = Mathf.Clamp(_zoom - move, 0f, 1f);

            cam.OffsetFromCharacter = Vector3.Lerp(zoomMin, zoomMax, curve.Evaluate(_zoom));
        }
    }
}

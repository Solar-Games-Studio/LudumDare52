using UnityEngine.UI;
using UnityEngine;

namespace Game.UI
{
    [ExecuteAlways]
    public class FlashUI : MonoBehaviour
    {
        [SerializeField] Graphic image;
        [SerializeField] Color color1 = Color.white;
        [SerializeField] Color color2 = Color.black;
        [SerializeField] AnimationCurve curve = AnimationCurve.Constant(0f, 1f, 0f);
        [SerializeField] float speed = 1f;

        private void Reset()
        {
            image = GetComponent<Graphic>();
        }

        private void Update()
        {
            float t = Time.unscaledTime;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                t = (float)UnityEditor.EditorApplication.timeSinceStartup;
#endif

            image.color = Color.Lerp(color1, color2, curve.Evaluate(t * speed));
        }
    }
}
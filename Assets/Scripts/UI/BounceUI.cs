using UnityEngine;

namespace Game.UI
{
    [ExecuteAlways]
    public class BounceUI : MonoBehaviour
    {
        [SerializeField] RectTransform target;
        [SerializeField] Vector3 size1;
        [SerializeField] Vector3 size2;
        [SerializeField] AnimationCurve curve;
        [SerializeField] float speed;

        private void Reset()
        {
            target = GetComponent<RectTransform>();
        }

        private void Update()
        {
            float t = Time.unscaledTime;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                t = (float)UnityEditor.EditorApplication.timeSinceStartup;
#endif

            target.localScale = Vector3.Lerp(size1, size2, curve.Evaluate(t * speed));
        }
    }
}

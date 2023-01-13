using UnityEngine;
using TMPro;
using System;
using System.Collections;

[Serializable]
public class Margin
{
    public float marginScaling = 0.1f;
    public float left;
    public float top;
    public float right;
    public float bottom;
    public Vector3 Get3DOffset()
    {
        return marginScaling * new Vector3(left - right, bottom - top);
    }
    public Vector2 GetDeltaSize()
    {
        return marginScaling * new Vector2(left + right, top + bottom);
    }
}

public class Bubble : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer background;
    [SerializeField]
    SpriteRenderer pointer;
    [SerializeField]
    TextMeshPro text;

    [SerializeField]
    Margin textMargin;

    [SerializeField]
    Vector2 rectScale;

    [SerializeField]
    float fadeInDuration;
    [SerializeField]
    float slideInDuration;

    [SerializeField]
    float backgroundOffset = -1.0f;
    [SerializeField]
    float pointerOffset = 1.0f;

    private delegate void ValueChangeDelegate(float t);

    private void Start()
    {
        rectScale = text.rectTransform.rect.size;
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        pointer.color = new Color(pointer.color.r, pointer.color.g, pointer.color.b, 0);
        SlideInRectangle();
    }
    private void Update()
    {
        text.rectTransform.anchoredPosition3D = background.transform.localPosition + textMargin.Get3DOffset();
        text.rectTransform.sizeDelta = rectScale * background.transform.localScale - textMargin.GetDeltaSize();
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
    }
    
    void SlideInRectangle()
    {
        AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, slideInDuration, 1.0f);
        Vector3 startRectPosition = background.transform.localPosition;
        ValueChangeDelegate onChange = (float t) =>
        {
            Color newColor = background.color;
            newColor.a = t;
            background.color = newColor;

            Vector3 offset = Vector3.Lerp(new Vector3(backgroundOffset, 0.0f, 0.0f), Vector3.zero, t);
            background.transform.localPosition = startRectPosition + offset;
        };
        StartCoroutine(AnimateFloat(curve, onChange, () => { FadeInText(); SlideInPointer(); }));
    }
    void FadeInText()
    {
        AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, fadeInDuration, 1.0f);
        ValueChangeDelegate onChange = (float t) =>
        {
            Color newColor = text.color;
            newColor.a = t;
            text.color = newColor;
        };
        StartCoroutine(AnimateFloat(curve, onChange));
    }
    
    void SlideInPointer()
    {
        AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, slideInDuration, 1.0f);
        Vector3 startPointerPosition = pointer.transform.localPosition;
        pointer.color = new Color(pointer.color.r, pointer.color.g, pointer.color.b, 1.0f);
        ValueChangeDelegate onChange = (float t) =>
        {
            Vector3 offset = Vector3.Lerp(new Vector3(0.0f, 1.0f, 0.0f), Vector3.zero, t);
            pointer.transform.localPosition = startPointerPosition + offset;
        };
        StartCoroutine(AnimateFloat(curve, onChange));
    }
    IEnumerator AnimateFloat(AnimationCurve curve, ValueChangeDelegate onValueChanged, Action onAnimationEnd = null)
    {
        for (float t = 0.0f; t <= curve.keys[^1].time; t += Time.deltaTime)
        {
            onValueChanged(curve.Evaluate(t));
            yield return new WaitForEndOfFrame();
        }
        onAnimationEnd?.Invoke();
    }


}

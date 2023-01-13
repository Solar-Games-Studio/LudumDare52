using UnityEngine;

public class BubbleFactory : MonoBehaviour
{
    private static BubbleFactory singleton;
    [SerializeField]
    Bubble bubblePrefab;

    private void Awake()
    {
        singleton = this;
    }

    public static BubbleFactory Singleton()
    {
        if (!singleton)
            Debug.LogError("Failed to create bubble! BubbleFactory object does not exist in scene");
        return singleton;
    }

    public static Bubble ShowBubbleOverObject(GameObject obj, float heightOffset)
    {
        Bubble bubble = Instantiate(Singleton().bubblePrefab);
        bubble.transform.position = obj.transform.position + heightOffset * Vector3.up;
        bubble.transform.SetParent(obj.transform);
        return bubble;
    }
    public static Bubble ShowBubbleInPosition(Transform bubblePosition)
    {
        Bubble bubble = Instantiate(Singleton().bubblePrefab);
        bubble.transform.position = bubblePosition.position;
        bubble.transform.SetParent(bubblePosition.transform);
        return bubble;
    }
}

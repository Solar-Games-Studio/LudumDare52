using UnityEngine;

public enum BubbleType
{
    Small,
    Big
}

public class BubbleFactory : MonoBehaviour
{
    private static BubbleFactory singleton;
    
    [SerializeField]
    SerializedDictionary<BubbleType, Bubble> bubblePrefabs;

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
    private Bubble GetBubblePrefab(BubbleType type)
    {
        bubblePrefabs.TryGetValue(type, out Bubble prefab);
        if (prefab == null)
            Debug.LogError($"'{type}' type Bubble prefab not found!");
        return prefab;
    }

    public static Bubble ShowBubbleOverObject(BubbleType type, GameObject obj, float heightOffset)
    {
        Bubble bubble = Instantiate(Singleton().GetBubblePrefab(type));
        bubble.transform.position = obj.transform.position + heightOffset * Vector3.up;
        bubble.transform.SetParent(obj.transform);
        bubble.Show();
        return bubble;
    }
    public static Bubble ShowBubbleOnTransform(BubbleType type, Transform bubblePosition)
    {
        Bubble bubble = Instantiate(Singleton().GetBubblePrefab(type));
        bubble.transform.position = bubblePosition.position;
        bubble.transform.SetParent(bubblePosition.transform);
        bubble.Show();
        return bubble;
    }
}

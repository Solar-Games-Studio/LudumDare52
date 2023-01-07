using UnityEngine;

namespace Game.Scene
{
    [CreateAssetMenu(fileName = "New Scene Preset", menuName = "Scriptable Objects/Scene/Preset")]
    public class ScenePreset : ScriptableObject
    {
        [SceneName] public string[] targetScenes;

        [SceneName] public string[] staticScenes;
    }
}
using UnityEngine;

namespace Game.Scene
{
    [CreateAssetMenu(fileName = "Scene Manager Data", menuName = "Scriptable Objects/Scene/Manager Data")]
    public class SceneManagerData : ScriptableObject
    {
        [SceneName] public string[] staticScenes;

        public ScenePreset defaultPreset;
        [EditorButton(nameof(ReloadManager))]
        public ScenePreset[] presets;

        void ReloadManager()
        {
            SceneManager.Reload();
        }
    }
}

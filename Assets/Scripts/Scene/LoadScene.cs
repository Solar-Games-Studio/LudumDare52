using UnityEngine;

namespace Game.Scene
{
    public class LoadScene : MonoBehaviour
    {
        [SerializeField] [SceneName] string scene;

        public void Load() =>
            SceneManager.LoadScene(scene);

        public void Load(string scene) =>
            SceneManager.LoadScene(scene);
    }
}
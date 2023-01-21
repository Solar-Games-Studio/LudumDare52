using UnityEngine;

using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Game.Scene
{
    internal class SceneManagerObject : MonoBehaviour
    {
        public string sceneName;

        private void Update()
        {
            USceneManager.SetActiveScene(USceneManager.GetSceneByName(sceneName));
            Destroy(gameObject);
        }
    }
}
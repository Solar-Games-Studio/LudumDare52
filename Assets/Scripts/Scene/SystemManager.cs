using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using qASIC;

namespace Game.Systems
{
    public class SystemManager : MonoBehaviour
    {
        const string SYSTEMS_SCENE_NAME = "Systems";

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void InitializeSystems()
        {
            if (!Application.CanStreamedLevelBeLoaded(SYSTEMS_SCENE_NAME))
            {
                qDebug.LogError("[SYSTEMS] Cannot initialize systems!");
                return;
            }

            SceneManager.LoadScene(SYSTEMS_SCENE_NAME, LoadSceneMode.Additive);
        }

        private void FixedUpdate()
        {
            List<GameObject> systems = new List<GameObject>();
            gameObject.scene.GetRootGameObjects(systems);
            if (systems.Count > 1) return;

            SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
        }
    }
}
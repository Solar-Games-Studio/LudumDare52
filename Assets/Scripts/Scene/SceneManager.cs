using UnityEngine;
using System.Collections.Generic;
using qASIC;
using UnityEngine.SceneManagement;
using System;

using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Game.Scene
{
    public static class SceneManager
    {
        #region Events
        public static event Action<string> OnSceneLoaded;
        #endregion

        #region Data
        const string ResourcesDataPath = "Scene/Scene Manager Data";

        static SceneManagerData _data = null;
        static SceneManagerData Data 
        { 
            get
            {
                if (_data == null)
                    _data = Resources.Load<SceneManagerData>(ResourcesDataPath);

                return _data;
            }
        }
        #endregion

        #region Presets
        private static Dictionary<string, ScenePreset> _scenePresets = null;
        public static Dictionary<string, ScenePreset> ScenePresets 
        { 
            get
            {
                if (_scenePresets == null)
                    ResetScenePresets();

                return _scenePresets;
            }
        }

        static void ResetScenePresets()
        {
            _scenePresets = new Dictionary<string, ScenePreset>();
            var data = Data;

            foreach (var preset in data.presets)
            {
                foreach (var scene in preset.targetScenes)
                {
                    if (_scenePresets.ContainsKey(scene))
                    {
                        qDebug.Log($"[Scene Manager] Scene {scene} is targetted multiple times in {preset.name} and {_scenePresets[scene].name}!", "Scene");
                        continue;
                    }

                    _scenePresets.Add(scene.ToLower(), preset);
                }
            }
        }

        public static ScenePreset GetPreset(string scene) =>
            ScenePresets.TryGetValue(scene.ToLower(), out ScenePreset preset) ? preset : Data.defaultPreset;
        #endregion

        #region Initialization
        public static void Reload()
        {
            ResetScenePresets();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            CurrentScene = UnitySceneManager.GetActiveScene().name;
            var data = Data;
            foreach (var staticScene in data.staticScenes)
                UnitySceneManager.LoadScene(staticScene, LoadSceneMode.Additive);

            CurrentPreset = GetPreset(CurrentScene);
            if (CurrentPreset != null)
                foreach (var scene in CurrentPreset.staticScenes)
                    UnitySceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
        #endregion

        #region Scenes
        public static string CurrentScene { get; private set; }
        public static ScenePreset CurrentPreset { get; private set; }

        public static void LoadScene(string scene)
        {
            if (!Application.CanStreamedLevelBeLoaded(scene))
                return;

            var preset = GetPreset(scene);

            var activeScenes = new List<string>();
            for (int i = 0; i < UnitySceneManager.sceneCount; i++)
                activeScenes.Add(UnitySceneManager.GetSceneAt(i).name);

            if (preset != CurrentPreset)
            {
                if (CurrentPreset != null)
                    foreach (var presetScene in CurrentPreset.staticScenes)
                        if (activeScenes.Contains(presetScene))
                            UnitySceneManager.UnloadSceneAsync(presetScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                if (preset != null)
                    foreach (var presetScene in preset.staticScenes)
                        UnitySceneManager.LoadScene(presetScene, LoadSceneMode.Additive);
            }

            UnitySceneManager.UnloadSceneAsync(CurrentScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            UnitySceneManager.LoadScene(scene, LoadSceneMode.Additive);
            new GameObject().AddComponent<SceneManagerObject>().sceneName = scene;

            CurrentScene = scene;
            CurrentPreset = preset;

            OnSceneLoaded?.Invoke(scene);
        }
        #endregion
    }
}
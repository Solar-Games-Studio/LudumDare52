using System.Collections.Generic;
using UnityEngine;
using qASIC.Console.Commands;
using Game.Scene;

namespace Game.Commands
{
    public class SceneCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "scene";
        public override string Description { get; } = "loads specified scene";
        public override string Help { get; } = "Use scene; scene <name>";
        public override string[] Aliases { get; } = new string[] { "loadscene", "level", "loadlevel" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 2)) return;
            switch (args.Count)
            {
                default:
                    LogScene();
                    return;
                case 2:
                    if (args[1].ToLower() == "reload")
                    {
                        ReloadScene();
                        return;
                    }

                    LoadScene(args[1]);
                    return;
            }
        }

        void LoadScene(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                LogError($"Scene '{sceneName}' does not exist!");
                return;
            }
            SceneManager.LoadScene(sceneName);
        }

        void LogScene() => Log($"Current scene: '{SceneManager.CurrentScene}'", "scene");

        void ReloadScene() => LoadScene(SceneManager.CurrentScene);
    }
}
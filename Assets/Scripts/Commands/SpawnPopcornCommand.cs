using System.Collections.Generic;
using qASIC.Console.Commands;
using Game.Player;
using Game.Harvestables.Materials;
using UnityEngine;

namespace Game.Commands
{
    public class SpawnPopcornCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "spawnpopcorn";
        public override string Description { get; } = "spawns popcorn";
        public override string Help => "Use spawnpopcorn; spawnpopcorn <material1> <material2> ...";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCountMin(args, 0)) return;

            var database = HarvestableMaterialDatabase.Instance;

            var materials = new List<HarvestableMaterial>();

            switch (args.Count)
            {
                case 1:
                    break;
                default:
                    for (int i = 1; i < args.Count; i++)
                    {
                        if (!database.MaterialsDictionary.ContainsKey(args[i]))
                        {
                            ParseException(args[i], "Harvestable Material");
                            return;
                        }

                        materials.Add(database.MaterialsDictionary[args[i]]);
                    }
                    break;
            }

            var player = PlayerReference.Singleton;

            if (player == null)
            {
                LogError("Player has not been initialized");
                return;
            }

            var popcorn = Object.Instantiate(database.Popcorn, player.transform.position, Quaternion.identity);
            popcorn.materials = materials;

            Log("Spawned popcorn", "cheat");
        }
    }
}
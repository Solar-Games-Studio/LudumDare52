using System.Collections;
using System.Collections.Generic;
using Game.Ordering;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class SpawnNPCCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "spawnnpc";
        public override string Description { get; } = "spawns a new NPC if there are any orders left";
    
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            var orderManager = OrderManager.Singleton;

            if (orderManager == null)
            {
                LogError("Order Manager has not been initialized!");
                return;
            }

            orderManager.SpawnNpc();
            Log("Force spawned NPC", "cheat");
        }
    }
}
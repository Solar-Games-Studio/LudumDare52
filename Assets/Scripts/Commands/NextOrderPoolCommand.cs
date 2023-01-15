using System.Collections.Generic;
using Game.Ordering;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class NextOrderPoolCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "nextorderpool";
        public override string Description { get; } = "skips to the next order pool";
        public override string[] Aliases => new string[] { "nextpool" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            var orderManager = OrderManager.Singleton;

            if (orderManager == null)
            {
                LogError("Order Manager has not been initialized!");
                return;
            }

            orderManager.NextPool();
            Log("Force skipped order pool", "cheat");
        }
    }
}
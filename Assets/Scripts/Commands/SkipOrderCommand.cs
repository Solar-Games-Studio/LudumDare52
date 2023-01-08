using System.Collections.Generic;
using qASIC.Console.Commands;
using Game.Ordering;

namespace Game.Commands
{
    public class SkipOrderCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "skiporder";
        public override string Description { get; } = "skips the current order";
        public override string[] Aliases { get; } = new string[] { "nextorder" };
    
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            var orderManager = OrderManager.Singleton;

            if (orderManager == null)
            {
                LogError("Order Manager has not been initialized!");
                return;
            }

            orderManager.NextOrder();
            Log("Force skipped order", "cheat");
        }
    }
}
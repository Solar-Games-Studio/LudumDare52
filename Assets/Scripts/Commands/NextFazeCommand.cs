using System.Collections.Generic;
using qASIC.Console.Commands;
using Game.Ordering;

namespace Game.Commands
{
    public class NextFazeCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "nextfaze";
        public override string Description { get; } = "starts the next faze";
        public override string Help => "Use faze";
        public override string[] Aliases { get; } = new string[] { "skipfaze" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            var orderManager = OrderManager.Singleton;

            if (orderManager == null)
            {
                LogError("Order Manager has not been initialized!");
                return;
            }

            orderManager.NextFaze();
            Log("Force started the next faze", "cheat");
        }
    }
}
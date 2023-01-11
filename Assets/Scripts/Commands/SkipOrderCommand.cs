using System.Collections.Generic;
using qASIC.Console.Commands;
using Game.Ordering;

namespace Game.Commands
{
    public class SkipOrderCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "finishorder";
        public override string Description { get; } = "finishes the order";
        public override string Help => "Use finishorder; finishorder <state>";
        public override string[] Aliases { get; } = new string[] { "fo" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            var orderManager = OrderManager.Singleton;
            var state = OrderManager.FinishState.Correct;

            switch (args.Count)
            {
                case 1:
                    break;
                case 2:
                    if (!System.Enum.TryParse(args[1], true, out state))
                    {
                        ParseException(args[1], nameof(OrderManager.FinishState));
                        return;
                    }
                    break;
            }

            if (orderManager == null)
            {
                LogError("Order Manager has not been initialized!");
                return;
            }

            orderManager.FinishOrder(state);
            Log("Force skipped order", "cheat");
        }
    }
}
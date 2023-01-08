using System.Collections;
using System.Collections.Generic;
using Game.Ordering;
using qASIC.Console.Commands;
using qASIC.Tools;

namespace Game.Commands
{
    public class OrderCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "order";
        public override string Description { get; } = "displays the current order";
    
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            var orderManager = OrderManager.Singleton;

            if (orderManager == null)
            {
                LogError("Order Manager has not been initialized!");
                return;
            }

            var order = orderManager.CurrentOrder;

            if (order == null)
            {
                Log("Current Order: null", "order");
                return;
            }

            var tree = new TextTree(qASIC.Console.GameConsoleController.GetConfig().textTreeStyle);
            var root = new TextTreeItem($"{order.name}:{order.GetInstanceID()}");

            var NPC = new TextTreeItem("NPC");
            NPC.Add($"{nameof(Order.canGetMad)}: {order.canGetMad}");
            NPC.Add($"{nameof(Order.canLeaveAbruptly)}: {order.canLeaveAbruptly}");
            root.Add(NPC);

            var time = new TextTreeItem("Time");
            if (order.canGetMad)
                time.Add($"{nameof(Order.satisfiedTime)}: {order.satisfiedTime}");
            if (order.canLeaveAbruptly)
                time.Add($"{nameof(Order.madTime)}: {order.madTime}");
            root.Add(time);

            var orders = new TextTreeItem("Orders");
            for (int i = 0; i < order.popcorns.Length; i++)
            {
                var popcorn = order.popcorns[i];

                var item = new TextTreeItem(i.ToString());
                item.Add($"{nameof(Order.Popcorn.amount)}: {popcorn.amount}");
                item.Add($"{nameof(Order.Popcorn.burnt)}: {popcorn.burnt}");
                var materials = new TextTreeItem("Materials");
                foreach (var material in popcorn.materials)
                    materials.Add(material.materialName);

                item.Add(materials);
                orders.Add(item);
            }

            root.Add(orders);
            Log(tree.GenerateTree(root), "order");
        }
    }
}
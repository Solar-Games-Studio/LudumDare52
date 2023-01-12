using Game.Interaction;
using Game.Inventory;
using System.Collections.Generic;
using Game.Ordering;
using Game.Player;
using Game.Harvestables;

namespace Game.Interactables
{
    public class Counter : Interactable, IItemInteractable
    {
        List<Order.Popcorn> _orders = new List<Order.Popcorn>();
        int count = 0;

        public bool ItemInteract()
        {
            var inventory = PlayerReference.Singleton.GetComponent<CharacterInventory>();
            var item = inventory.RemoveItem();


            if (item is PopCornObject popcorn)
            {
                _orders.Add(new Order.Popcorn()
                {
                    amount = 1,
                    burnt = popcorn.burned,
                    materials = popcorn.materials,
                });
            }

            Destroy(item.gameObject);

            if (OrderManager.Singleton?.CurrentOrder == null)
                return false;

            count++;
            if (OrderManager.Singleton.CurrentOrderItemAmount > count)
                return false;

            FinalizeOrder();

            return false;
        }

        void FinalizeOrder()
        {
            count = 0;

            bool isOrderCorrect = IsOrderCorrect();

            OrderManager.Singleton.FinishOrder();
            OrderManager.Singleton.NextOrder();
        }

        bool IsOrderCorrect()
        {
            var orderPopcorns = OrderManager.Singleton.CurrentOrder.popcorns;
            foreach (var orderPopcorn in orderPopcorns)
            {
                int amount = 0;
                foreach (var counterPopcorn in _orders)
                {
                    if (orderPopcorn.burnt != counterPopcorn.burnt) continue;
                    if (!orderPopcorn.CompareMaterials(counterPopcorn.materials)) continue;
                    amount++;
                }

                if (amount != orderPopcorn.amount)
                    return false;
            }

            return true;
        }
    }
}

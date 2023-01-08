using UnityEngine;
using System.Collections.Generic;
using qASIC;

namespace Game.Ordering
{
    public class OrderManager : MonoBehaviour
    {
        [EditorButton(nameof(NextOrder))]
        [EditorButton(nameof(NextPool))]
        public OrderPool[] poolTimeline;

        public static OrderManager Singleton { get; private set; }

        public int CurrentPool { get; private set; } = -1;
        public int CurrentPoolOrder { get; private set; } = -1;
        public Order CurrentOrder { get; private set; }
        public int CurrentOrderItemAmount { get; private set; }

        Queue<Order> _orders = new Queue<Order>();

        private void Awake()
        {
            Singleton = this;
        }

        public void NextPool()
        {
            CurrentPool = Mathf.Min(CurrentPool + 1, poolTimeline.Length - 1);

            var pool = poolTimeline[CurrentPool];
            Debug.Assert(pool.orders.Length > 0, "Order queue cannot be empty!");
            Debug.Assert(pool.orderAmount > 0, "Order amount has to be above 0!");

            CurrentPoolOrder = -1;

            var orderList = new List<Order>();

            int randomOrderCount = pool.orderAmount - pool.oneTimeOrders.Length;

            for (int i = 0; i < randomOrderCount; i++)
                orderList.Add(pool.orders[Random.Range(0, pool.orders.Length)]);

            foreach (var item in pool.oneTimeOrders)
                orderList.Insert(Random.Range(0, orderList.Count), item);

            _orders = new Queue<Order>(orderList);

            qDebug.Log($"[Order Manager] Pool finished, moved to the next pool '{pool.name}:{pool.GetInstanceID()}'", "order");
        }

        public void FinishOrder(bool wasCorrect)
        {
            qDebug.Log($"[Order Manager] Order finished, was correct: {wasCorrect}", "order");
        }

        public void NextOrder()
        {
            if (_orders.Count == 0)
            {
                NextPool();
                NextOrder();
                return;
            }

            CurrentPoolOrder++;
            CurrentOrder = _orders.Dequeue();
            CurrentOrderItemAmount = 0;

            foreach (var item in CurrentOrder.popcorns)
                CurrentOrderItemAmount += item.amount;

            qDebug.Log($"[Order Manager] Moved to the next order '{CurrentOrder.name}:{CurrentOrder.GetInstanceID()}'", "order");
        }
    }
}
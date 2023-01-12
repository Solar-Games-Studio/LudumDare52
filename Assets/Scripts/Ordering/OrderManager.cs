using UnityEngine;
using System.Collections.Generic;
using qASIC;
using Game.NPCs;

namespace Game.Ordering
{
    public class OrderManager : MonoBehaviour
    {
        public enum OrderState { None, Waiting, Mad, }
        public enum FinishState { Correct, Left, }

        [Label("Visual")]
        [SerializeField] NPC defaultNpcModel;
        [SerializeField] NPCSpawner npcSpawner;
        [SerializeField] float dialogueTimeLength;

        [Label("Ordering")]
        [EditorButton(nameof(SpawnNpc))]
        [EditorButton(nameof(FinishOrder))]
        [EditorButton(nameof(NextOrder))]
        [EditorButton(nameof(NextPool))]
        public OrderPool[] poolTimeline;

        public static OrderManager Singleton { get; private set; }

        public int CurrentPool { get; private set; } = -1;
        public int CurrentPoolOrder { get; private set; } = -1;
        public Order CurrentOrder { get; private set; }
        public int CurrentOrderItemAmount { get; private set; }
        public float OrderStartTime { get; private set; }
        public OrderState State { get; private set; }

        List<Order> _orders = new List<Order>();

        List<NPC> _npcs = new List<NPC>();


        private void Awake()
        {
            Singleton = this;
            NextPool();
        }

        private void FixedUpdate()
        {
            switch (State)
            {
                case OrderState.Waiting:
                    if (CurrentOrder.canGetMad && Time.time - OrderStartTime >= CurrentOrder.satisfiedTime)
                        GetMad();
                    break;
                case OrderState.Mad:
                    if (CurrentOrder.canLeaveAbruptly && Time.time - OrderStartTime >= CurrentOrder.satisfiedTime + CurrentOrder.madTime)
                        LeaveAbruptly();
                    break;
            }
        }

        public void NextPool()
        {
            CurrentPool = Mathf.Min(CurrentPool + 1, poolTimeline.Length - 1);

            var pool = poolTimeline[CurrentPool];
            Debug.Assert(pool.orders.Length > 0, "Order queue cannot be empty!");
            Debug.Assert(pool.orderAmount > 0, "Order amount has to be above 0!");

            CurrentPoolOrder = -1;

            _orders.Clear();

            int randomOrderCount = pool.orderAmount - pool.oneTimeOrders.Length;

            for (int i = 0; i < randomOrderCount; i++)
                _orders.Add(pool.orders[Random.Range(0, pool.orders.Length)]);

            foreach (var item in pool.oneTimeOrders)
                _orders.Insert(Random.Range(0, _orders.Count), item);

            qDebug.Log($"[Order Manager] Pool finished, moved to the next pool '{pool.name}:{pool.GetInstanceID()}'", "order");
        }

        public void FinishOrder(FinishState finishState = FinishState.Correct)
        {
            if (CurrentOrder == null)
                return;

            qDebug.Log($"[Order Manager] Order finished, finish state: {finishState}", "order");

            if (_npcs.Count > 0)
            {
                string[] dialogueoptions = finishState switch
                {
                    FinishState.Correct => State == OrderState.Mad ? CurrentOrder.madDialogue : CurrentOrder.exitDialogue,
                    FinishState.Left => CurrentOrder.abruptExitDialogue,
                    _ => new string[0],
                };

                string dialogue = dialogueoptions.Length == 0 ?
                    "[Order] There was an error retrieving the correct dialogue set" :
                    dialogueoptions[Random.Range(0, dialogueoptions.Length)];

                _npcs[0].DisplayDialogue(dialogue, dialogueTimeLength);
                _npcs[0].GoAway();
                _npcs.RemoveAt(0);
            }

            State = OrderState.None;
        }

        void GetMad()
        {
            qDebug.Log($"[Order Manager] Order state has been changed to mad", "order");
            State = OrderState.Mad;
        }

        public void LeaveAbruptly()
        {
            State = OrderState.None;
            FinishOrder(FinishState.Left);
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

            CurrentOrder = _orders[0];
            _orders.RemoveAt(0);

            CurrentOrderItemAmount = 0;
            OrderStartTime = Time.time;

            foreach (var item in CurrentOrder.popcorns)
                CurrentOrderItemAmount += item.amount;

            qDebug.Log($"[Order Manager] Moved to the next order '{CurrentOrder.name}:{CurrentOrder.GetInstanceID()}'", "order");
        }

        public void SpawnNpc()
        {
            if (_orders.Count < _npcs.Count)
                return;

            var order = _orders[_npcs.Count];
            var npc = npcSpawner.SummonNPC(order.model ?? defaultNpcModel);
            npc.OnExit.AddListener(NPC_OnExit);
            npc.OnNewCustomerArrived.AddListener(NPC_OnNewCustomerArrived);

            _npcs.Add(npc);
        }

        void NPC_OnExit(NPC npc)
        {
            _npcs.Remove(npc);
        }

        void NPC_OnNewCustomerArrived()
        {
            NextOrder();
        }
    }
}
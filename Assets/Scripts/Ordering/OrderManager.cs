using UnityEngine;
using System.Collections.Generic;
using qASIC;
using Game.NPCs;
using Game.Harvestables.Materials;

namespace Game.Ordering
{
    public class OrderManager : MonoBehaviour
    {
        public enum OrderState { None, Waiting, Mad, }
        public enum FinishState { Correct, Left, }

        [Label("Fazes")]
        [SerializeField][ReorderableList] Faze[] fazes;

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
        public float OrderStartTime { get; private set; }
        public OrderState State { get; private set; }


        List<Order> _orders = new List<Order>();

        List<NPC> _npcs = new List<NPC>();

        int _faze = -1;
        float _fazeStartTime;
        float _fazeTimeLength;
        float _npcLastSpawnTime;
        float _npcSpawnTimeLength;


        private void Awake()
        {
            Singleton = this;
            NextFaze();
            NextPool();
        }

        private void Update()
        {
            var faze = fazes[_faze];

            if (faze.spawnNPCs && Time.time - _npcLastSpawnTime >= _npcSpawnTimeLength)
                SpawnNpc();

            if (faze.hasTimeLimit && Time.time - _fazeStartTime >= _fazeTimeLength)
                NextFaze();
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

        #region Fazes
        public void NextFaze()
        {
            _faze = Mathf.Min(_faze + 1, fazes.Length - 1);
            var faze = fazes[_faze];

            _fazeStartTime = Time.time;
            _fazeTimeLength = Random.Range(faze.timeLimit.x, faze.timeLimit.y);

            ResetNPCTimer(true);

            qDebug.Log($"[Faze] Started next faze id:{_faze}", "faze");
        }
        #endregion

        #region Orders
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

        void FinishOrder() =>
            FinishOrder(FinishState.Correct);

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
                npcSpawner.NextCustomer();
                _npcs.RemoveAt(0);
            }

            State = OrderState.None;
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

            OrderStartTime = Time.time;

            Preparation = new List<PreparationItem>();
            foreach (var item in CurrentOrder.popcorns)
                for (int i = 0; i < item.amount; i++)
                    Preparation.Add(new PreparationItem(item));

            qDebug.Log($"[Order Manager] Moved to the next order '{CurrentOrder.name}:{CurrentOrder.GetInstanceID()}'", "order");
        }
        #endregion

        #region Preparation
        public List<PreparationItem> Preparation { get; set; }

        public void FinishPreparingItem(PreparationItem item)
        {
            int targetIndex = -1;

            for (int i = 0; i < Preparation.Count; i++)
            {
                if (!Preparation[i].IsEqual(item))
                    continue;

                targetIndex = i;
                break;
            }

            if (targetIndex == -1)
            {
                //Wrong
                return;
            }

            Preparation.RemoveAt(targetIndex);

            if (Preparation.Count == 0)
            {
                FinishOrder();
            }
        }
        #endregion

        #region NPC
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

        public void SpawnNpc()
        {
            if (_orders.Count <= _npcs.Count)
                return;

            var order = _orders[_npcs.Count];
            var npc = npcSpawner.SummonNPC(order.model ?? defaultNpcModel);
            npc.OnExit.AddListener(NPC_OnExit);
            npc.OnNewCustomerArrived.AddListener(NPC_OnNewCustomerArrived);

            _npcs.Add(npc);
            ResetNPCTimer();

            qDebug.Log("[Order Manager] NPC spawned", "npc");
        }

        void ResetNPCTimer(bool first = false)
        {
            var faze = fazes[_faze];
            _npcLastSpawnTime = Time.time;

            _npcSpawnTimeLength = first ? 
                faze.firstNPCSpawnTime : 
                Random.Range(faze.NPCSpawnTime.x, faze.NPCSpawnTime.y);
        }

        void NPC_OnExit(NPC npc)
        {
            _npcs.Remove(npc);
        }

        void NPC_OnNewCustomerArrived()
        {
            NextOrder();
        }
        #endregion

        [System.Serializable]
        class Faze
        {
            [Label("Time")]
            public bool hasTimeLimit = true;
            [MinMaxSlider(2f, 180f)][HideIf(nameof(hasTimeLimit), false)] public Vector2 timeLimit = new Vector2(60f, 60f);

            [Label("Spawning")]
            public bool spawnNPCs;
            [HideIf(nameof(spawnNPCs), false)] public float firstNPCSpawnTime;
            [MinMaxSlider(2f, 180f)][HideIf(nameof(spawnNPCs), false)] public Vector2 NPCSpawnTime;
        }

        public class PreparationItem
        {
            public PreparationItem() { }

            public PreparationItem(Order.Popcorn popcorn)
            {
                burned = popcorn.burnt;
                materials = new List<HarvestableMaterial>(popcorn.materials);
            }

            public bool burned;
            public List<HarvestableMaterial> materials = new List<HarvestableMaterial>();

            public bool IsEqual(PreparationItem item)
            {
                if (item == null) return false;

                List<HarvestableMaterial> noncomparedMaterials = new List<HarvestableMaterial>(materials);
                bool containsSameMaterials = true;

                foreach (var material in item.materials)
                {
                    if (!noncomparedMaterials.Contains(material))
                    {
                        containsSameMaterials = false;
                        break;
                    }

                    noncomparedMaterials.RemoveAt(noncomparedMaterials.IndexOf(material));
                }

                if (containsSameMaterials)
                    containsSameMaterials = noncomparedMaterials.Count == 0;

                return burned == item.burned &&
                    containsSameMaterials;
            }
        }
    }
}
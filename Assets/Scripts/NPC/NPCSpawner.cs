using UnityEngine;
using System.Collections.Generic;

namespace Game.NPCs
{
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField]
        Transform sellPoint;
        [SerializeField]
        Transform exitPoint;
        [SerializeField]
        float betweenDistance = 0.2f;
        [SerializeField]
        float betweenDistanceMaxDeviation = 0.3f;
        [SerializeField]
        float NPCSpeed = 3.0f;
        [SerializeField]
        float NPCSpeedMaxDeviation = 1.0f;

        Queue<NPC> customerQueue;
        NPC lastCustomer;

        private void Start()
        {
            customerQueue = new Queue<NPC>();
        }

        /// <returns>Spawned SCP</returns>
        public NPC SummonNPC(NPC prefab)
        {
            var spawnedNPCObject = Instantiate(prefab);
            var spawnedNPC = spawnedNPCObject.GetComponent<NPC>();
            spawnedNPC.transform.position =
                new Vector3(transform.position.x,
                            spawnedNPC.transform.position.y,
                            transform.position.z);
            spawnedNPC.SetBetweenDistance(betweenDistance + Random.Range(-betweenDistanceMaxDeviation, betweenDistanceMaxDeviation));
            spawnedNPC.SetSpeed(NPCSpeed + Random.Range(-NPCSpeedMaxDeviation, NPCSpeedMaxDeviation));
            spawnedNPC.SetSellPoint(sellPoint);
            spawnedNPC.SetExitPoint(exitPoint);

            if (customerQueue.Count > 0)
                spawnedNPC.SetTarget(lastCustomer.transform);
            else
            {
                spawnedNPC.SetTarget(sellPoint);
                spawnedNPC.SetBetweenDistance(betweenDistance);
            }

            customerQueue.Enqueue(spawnedNPC);
            lastCustomer = spawnedNPC;

            return lastCustomer;
        }

        void NextCustomer()
        {
            customerQueue.Dequeue().GoAway();
            if (customerQueue.Count > 0)
                customerQueue.Peek().SetTarget(sellPoint);
        }
    }
}
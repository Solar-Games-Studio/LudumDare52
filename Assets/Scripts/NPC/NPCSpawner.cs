using UnityEngine;
using System.Collections.Generic;

namespace Game.NPC
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

        [SerializeField]
        [EditorButton(nameof(NextCustomer))]
        [EditorButton(nameof(SummonNPC))]
        GameObject[] NPCPrefabs;

        Queue<NPC> customerQueue;
        NPC lastCustomer;

        private void Start()
        {
            customerQueue = new Queue<NPC>();
        }

        void SummonNPC()
        {
            var spawnedNPCObject = Instantiate(DrawNPCPrefab());
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
        }

        void NextCustomer()
        {
            customerQueue.Dequeue().GoAway();
            if (customerQueue.Count > 0)
                customerQueue.Peek().SetTarget(sellPoint);
        }

        GameObject DrawNPCPrefab()
        {
            int index = Random.Range(0, NPCPrefabs.Length);
            return NPCPrefabs[index];
        }

    }
}
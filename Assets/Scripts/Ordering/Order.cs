using Game.Harvestables.Materials;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ordering
{
    [CreateAssetMenu(fileName = "New Order", menuName = "Scriptable Objects/Ordering/Order")]
    public class Order : ScriptableObject
    {
        public float weight = 1f;

        [Label("NPC")]
        public bool canGetMad;
        public bool canLeaveAbruptly;

        [Label("Time")]
        [Help("'satisfiedTime' -> *NPC getting mad* -> 'madTime' -> *NPC leaving*")]
        [HideIf(nameof(canGetMad), false)] public float satisfiedTime;
        [HideIf(nameof(canLeaveAbruptly), false)] public float madTime;

        [Label("Order")]
        public Popcorn[] popcorns;

        [Label("Dialogue")]
        public string[] enterDialogue;
        public string[] exitDialogue;

        [Space]
        [HideIf(nameof(canGetMad), false)] public string[] madDialogue;
        [HideIf(nameof(canGetMad), false)] public string[] madExitDialogue;
        [HideIf(nameof(canLeaveAbruptly), false)] public string[] abruptExitDialogue;

        [System.Serializable]
        public class Popcorn
        {
            public int amount;
            public bool burnt;
            public List<HarvestableMaterial> materials;

            public int GetMaterialCount(HarvestableMaterial material)
            {
                int count = 0;
                foreach (var item in materials)
                    if (item == material)
                        count++;

                return count;
            }

            public bool CompareMaterials(List<HarvestableMaterial> toCompare)
            {
                var own = new List<HarvestableMaterial>(materials);

                if (toCompare.Count != own.Count)
                    return false;

                foreach (var item in toCompare)
                {
                    if (!own.Contains(item)) return false;
                    own.Remove(item);
                }

                return true;
            }
        }
    }
}
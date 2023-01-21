using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.Inventory;
using qASIC;

namespace Game.Harvestables.Materials
{
    public class HarvestableMaterialDatabase : MonoBehaviour
    {
        [EditorButton(nameof(ReloadDatabase))]
        [SerializeField] List<HarvestableMaterial> materials;
        [SerializeField] [Prefab] PopCornObject popcorn;

        public static HarvestableMaterialDatabase Instance { get; private set; }
        public List<HarvestableMaterial> Materials => materials;
        public PopCornObject Popcorn => popcorn;
        public Dictionary<string, HarvestableMaterial> MaterialsDictionary { get; private set;} = new Dictionary<string, HarvestableMaterial>();

        private void Awake()
        {
            ReloadDatabase();
        }

        void ReloadDatabase()
        {
            Instance = this;
            MaterialsDictionary = materials
                .Where(x => x != null)
                .ToDictionary(x => x.materialName);
        }
    }
}
using UnityEngine;

namespace Game.Harvestables.Materials
{
    [CreateAssetMenu(fileName = "New Harvestable Material", menuName = "Scriptable Objects/Harvestable/Materials")]
    public class HarvestableMaterial : ScriptableObject
    {
        public string materialName;
    }
}
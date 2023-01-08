using UnityEngine;

namespace Game.Harvestables.Materials
{
    [CreateAssetMenu(fileName = "New Harvestable Material", menuName = "Scriptable Objects/Harvestable/Material")]
    public class HarvestableMaterial : ScriptableObject
    {
        public string materialName;
        public Sprite image;

        public float growTime;
    }
}
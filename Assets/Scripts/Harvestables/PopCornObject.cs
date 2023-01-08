using Game.Inventory;
using Game.Harvestables.Materials;
using System.Collections.Generic;

namespace Game.Harvestables
{
    public class PopCornObject : ItemObject
    {
        public bool burned;
        public List<HarvestableMaterial> materials = new List<HarvestableMaterial>();
    }
}
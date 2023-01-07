using UnityEngine;

namespace Game.Inventory
{
    public interface IItemHolder
    {
        public Transform HolderTransform { get; }
    }
}
using Game.Interaction;
using UnityEngine;

namespace Game.Inventory
{
    public class ItemHolderInteractable : Interactable, IItemHolder
    {
        [SerializeField] Transform itemHolder;

        public Transform HolderTransform =>
            itemHolder;
    }
}

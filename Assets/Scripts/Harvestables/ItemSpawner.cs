using Game.Interaction;
using Game.Inventory;
using Game.Player;
using UnityEngine;

namespace Game.Harvestables
{
    public class ItemSpawner : Interactable
    {
        [SerializeField] ItemObject item;

        public override void Interact()
        {
            base.Interact();
            var inventory = PlayerReference.Singleton.GetComponent<CharacterInventory>();

            var obj = Instantiate(item);
            inventory.EquipItem(obj);
        }
    }
}
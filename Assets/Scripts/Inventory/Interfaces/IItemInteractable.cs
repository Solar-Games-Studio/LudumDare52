namespace Game.Inventory
{
    public interface IItemInteractable
    {
        /// <summary>Called when the player interacts while holding an item</summary>
        /// <returns>Returns if the item can be dropped</returns>
        public bool ItemInteract();

        public bool CanDisplayDropPrompt();
    }
}
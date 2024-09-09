using static InventoryDiablo.ItemGrid;

namespace InventoryDiablo
{
    public interface IInventoryUI
    {
        public void CreateAndInsertItem(InventoryItem inventoryItem, ItemGrid grid);
        public void SetInventoryOwner(IInventorySystem newOwner);
        public void DestroyInventoryItem(InventoryItem inventoryItem);
        public void DestroyAllInventoryItem();
        public void ShowInventory();

        public void RefreshUI();
        public ItemGrid CheckFreeSpaceForItem(InventoryItem item);
    }
}

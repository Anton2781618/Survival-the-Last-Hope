using static InventoryDiablo.ItemGrid;

namespace InventoryDiablo
{
    public interface IInventoryUI
    {
        public void CreateAndInsertItem(InventoryItem inventoryItem, GridName gridName);
        public void SetInventoryOwner(IInventorySystem newOwner);
        public void DestroyInventoryItem(InventoryItem inventoryItem);
        public void ShowInventory();

        public void RefreshUI();
    }
}

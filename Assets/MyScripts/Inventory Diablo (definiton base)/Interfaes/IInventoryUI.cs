using static InventoryDiablo.UIItemGrid;

namespace InventoryDiablo
{
    public interface IInventoryUI
    {
        public void CreateAndInsertItem(InventoryItem inventoryItem, UIItemGrid grid);
        public void SetInventoryOwner(IInventorySystem newOwner);
        public void DestroyInventoryItem(InventoryItem inventoryItem);
        public void DestroyAllInventoryItem();
        public void ShowInventory(bool value);
        public void TurnInventory();

        public void RefreshUI();
        public UIItemGrid CheckFreeSpaceForItem(InventoryItem item);
    }
}

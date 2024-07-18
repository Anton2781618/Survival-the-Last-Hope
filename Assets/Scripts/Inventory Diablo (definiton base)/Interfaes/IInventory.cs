using static InventoryDiablo.ItemData;

namespace InventoryDiablo
{
    public interface IInventory
    {
        public void AddItem(InventoryItem item);
        public void RemoveItem(InventoryItem item);
        public InventoryItem TakeTtem(InventoryItem model,ItemData.ItemType itemType);
        public void ShowInventory();
    }
    
}

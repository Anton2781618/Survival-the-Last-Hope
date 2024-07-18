namespace InventoryDiablo
{
    public interface IInventorySystem
    {
        public IInventoryController InventoryController { get; }

        public void EquipItem(InventoryItem item);
        public void TakeOffItem(UIInventoryItem item);
        public void DropItem(InventoryItem item);
    }
}

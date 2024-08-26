namespace InventoryDefinition
{
    public interface IInventoryUI 
    {
        public void SetInventoryOwner(IInventorySystem newOwner);
        public void RefreshInventoryItems();
    }
}

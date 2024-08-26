namespace InventoryDefinition
{
    public interface IInventorySystem
    {
        public IInventoryHendler Inventory { get; }
        public IInventoryUI InventoryUI { get; }
        
        //взять предмет в руки
        public void EquipInventoryItem(Item item);
        public void ToDressUp(Item item);
        
    }
}

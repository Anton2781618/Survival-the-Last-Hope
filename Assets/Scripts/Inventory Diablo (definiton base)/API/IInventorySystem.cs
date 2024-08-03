namespace InventoryDiablo
{
    /// <summary>
    /// Интерфейс для работы с инвентарем. объекты, которые имплементируют этот интерфейс могут работать с инвентарем
    /// </summary>
    public interface IInventorySystem
    {
        public InventoryHandler InventoryHandler { get; }

        public void EquipItem(InventoryItem item);
        public void TakeOffItem(UIInventoryItem item);
        public void DropItem(InventoryItem item);
    }
}

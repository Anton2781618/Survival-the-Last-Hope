using System;

namespace InventoryDiablo
{
    public interface IInventoryController
    {
        public IInventory Inventory { get; set; }
        public IInventoryUI InventoryUI { get; }
    }
    
}

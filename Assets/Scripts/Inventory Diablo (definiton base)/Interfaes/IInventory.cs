using System;

namespace InventoryDiablo
{
    public interface IInventory
    {
        public Inventory Inventory {get; set;}
        public IInventoryUI InventoryUI {get;}
    }
    
}

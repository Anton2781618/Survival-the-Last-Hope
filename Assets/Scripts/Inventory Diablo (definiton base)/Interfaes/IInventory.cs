using System;

namespace InventoryDiablo
{
    public interface IInventory
    {
        public Inventory Inventory {get;}
        public IInventoryUI InventoryUI {get;}
    }
    
}

using System;

namespace InventoryDiablo
{
    public interface IInventoryController
    {
        public IInventory Inventory {get;}
        public IInventoryUI InventoryUI {get;}
    }
    
}

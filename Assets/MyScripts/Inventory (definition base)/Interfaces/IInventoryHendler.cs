using System.Collections.Generic;

namespace InventoryDefinition
{
    public interface IInventoryHendler
    {
        public void AddItem(Item item);

        public void RemoveItem(Item item);

        public List<Item> GetItemList();
    }
}

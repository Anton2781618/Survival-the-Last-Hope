using System.Collections.Generic;
using UnityEngine;

namespace InventoryDefinition
{
    public class InventoryHandler : IInventoryHendler
    {
        private List<Item> itemList = new List<Item>();

        // метод положить в инвентарь
        public void AddItem(Item item)
        {
            if(item.IsStackable())
            {
                bool itemAlreadyInInventory = false;
                
                foreach (Item inventoryItem in itemList)
                {
                    if (inventoryItem.itemType == item.itemType)
                    {
                        inventoryItem.amount += item.amount;

                        itemAlreadyInInventory = true;
                    }
                }

                if (!itemAlreadyInInventory)
                {
                    itemList.Add(item);
                }

                return; 
            }

            itemList.Add(item);
        }

        //достать из инвентаря
        public void RemoveItem(Item item)
        {
            if (itemList.Contains(item))
            {
                itemList.Remove(item);
            }
        }

        public List<Item> GetItemList()
        {
            return itemList;
        }
    }
}
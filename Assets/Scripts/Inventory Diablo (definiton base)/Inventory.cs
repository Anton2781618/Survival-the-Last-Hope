using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventoryDiablo
{

    //класс является представлением места для хранения предметов (сундук или инвентарь игрока или торговца)
    [Serializable]
    public class Inventory
    {
        public int money = 500;
        [SerializeField] private List<InventoryItem> inventoryItems = new List<InventoryItem>();

        public void OpenChest()
        {
            ClearItems();
        }

        //вставить все итемы в инвентарь
        public void UpdateChestItems()
        {

            
        }

        //получить итем из мнвентаря
        public InventoryItem GetInventoryItem(ItemData itemData)
        {
            foreach (var item in inventoryItems)
            {
                if(item.ItemData == itemData) return item;
            }

            return null;
        }

        public List<InventoryItem> GetInventoryItems() => inventoryItems;

        //проверяет есть ли в инвентаре такой предмет по scriptable object
        public bool CheckInventoryForItems(ItemData itemData)
        {
            foreach (var item in inventoryItems)
            {
                if(item.ItemData == itemData) return true;
            }

            return false;
        }

        //проверяет есть ли в инвентаре такой предмет по типу
        public int CheckInventoryForItemsType(ItemData.ItemType itemType)
        {
            foreach (var item in inventoryItems)
            {
                if(itemType.HasFlag(item.ItemData.TypeItem)) return 0;
            }

            return 1;
        }

        //взять любой предмет инвентаря по типу предмета 
        public InventoryItem GetInventoryForItemType(ItemData.ItemType itemType)
        {
            foreach (var item in inventoryItems)
            {
                if(itemType.HasFlag(item.ItemData.TypeItem)) return item;
            }

            return null;
        }

        //добавить предмет в сундук предмет по  
        public void AddItem(InventoryItem item)
        {
            Debug.Log($"Добавлен предмет в инвентарь {item.ItemData.Title}");

            inventoryItems.Add(item);
        }

        //добавить предмет в сундук по scriptable object
        public void AddItemToChest(ItemData itemData) => inventoryItems.Add(new InventoryItem(itemData, itemData.Benefit));

        // метод убирает из списка итемов в инвентаре определенный итем 
        public void RemoveItem(InventoryItem item)
        {
            inventoryItems.Remove(item);
            // for (int i = 0; i < inventoryItems.Count; i++)
            // {
            //     if(inventoryItems[i].ItemData == item.ItemData && inventoryItems[i].Amount == item.Amount)
            //     {
            //         inventoryItems.RemoveAt(i);

            //         return;
            //     }
            // }
        }

        public InventoryItem TakeTtem(InventoryItem model,ItemData.ItemType itemType)
        {
            foreach (InventoryItem item in inventoryItems)
            {
                //проверить на соответствие типа и так что бы item.ItemData был в model.ItemData.canBeCombined
                if(item.ItemData.TypeItem.HasFlag(itemType) && model.ItemData.CanBeCombined.Contains(item.ItemData) && item.Amount > 0)
                {
                    return item;
                }
            }

            return null;
        }

        public void RemoveAtChestGrid(InventoryItem item)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if(inventoryItems[i].ItemData == item.ItemData && inventoryItems[i].Amount == item.Amount)
                {
                    inventoryItems.RemoveAt(i);
                    return;
                }
            }
        }

        //удалить UI объекты из слоя инвентаря
        private void ClearItems()
        {
            inventoryItems.Clear();
        }

        public void ShowInventory()
        {
            Debug.Log($"в инвентаре {inventoryItems.Count} экземпляров");

            foreach (var item in inventoryItems)
            {
                Debug.Log($"{item.ItemData.Title} внутри итема {item.CombinedItems.Count} предметов");

                foreach (var combinedItem in item.CombinedItems)
                {
                    Debug.Log(item.ItemData.Title + " " + combinedItem.Value.ItemData.Title + " " + combinedItem.Value.Amount);
                }
                Debug.Log("______________________________");
            }
        }

        //взять итемы из списка и создать физически
        private void InsertAllInventoryItems()
        {
            // foreach (InventoryItemInfo item in inventoryItems)
            // {
            //     inventoryController.CreateAndInsertItem(item.itemData, _chestGrid, item.Amount);
            // }
        
            // foreach (var item in Clothes.items)
            // {
            //     if(item.Prefab != null)
            //     {
            //         // foreach (var grid in GameManager.Instance.UIManager.GetPlayerInventoryWindowUI().GetPlayerInventoryGrids())
            //         // {
            //         //     if(grid.GetGridForItemsType() == item.ItemType)
            //         //     {
            //         //         inventoryController.CreateAndInsertItem(item.Prefab.GetItemData(), grid, 0);
            //         //     }
            //         // }
            //     }
            // }
        }

        
    }
}
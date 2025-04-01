using System;
using System.Collections.Generic;
using ModularEventArchitecture;
using UnityEngine;

namespace InventoryDiablo
{
    //класс является представлением места для хранения предметов (сундук или инвентарь игрока или торговца)
    [Serializable]
    public class Inventory
    {
        //---------------------------------------------------
        public GameEntity Entity{get; set;}
        //---------------------------------------------------
        public List<InventoryContainer> InventoryContainers = new List<InventoryContainer>();
        //---------------------------------------------------
        //Размер окна инвентаря
        public Vector2 InventoryWindowSize = new Vector2(500, 500);

        //!---------------------------------------------------

        //взять любой предмет инвентаря по типу предмета 
        public InventoryItem GetInventoryForItemType(ItemData.ItemType itemType)
        {
            // foreach (var item in inventoryItems)
            // {
            //     if(itemType.HasFlag(item.ItemData.TypeItem)) return item;
            // }

            return null;
        }

        //добавить предмет в сундук предмет по  
        public void AddItem(InventoryItem item)
        {
            // Debug.Log($"Добавлен предмет в инвентарь {item.ItemData.Title}");

            // inventoryItems.Add(item);
        }

        public bool TryPlaceItemInInventory(InventoryItem item)
        {
            if(TryPlaceItemInSlots(item)) return true;

            if(TryPlaceItemInGridsSlots(item)) return true;

            return false;
        }

        private bool TryPlaceItemInSlots( InventoryItem item)
        {
            foreach (var container in InventoryContainers)
            {
                foreach (var slot in container.Slots)
                {
                    if(slot.SlotGrid.TryPlaceItem(item))
                    {
                        Entity.LocalEvents.Publish(EventsInventory.Equip_item_in_slot, new EquipItemEventData
                        {
                            InventoryItem = item,
                            Parent = slot.PlaceToSpawnObject.gameObject,
                            Slot = slot

                        });

                        return true;
                    } 
                }
            }

            return false;
        }

        private bool TryPlaceItemInGridsSlots(InventoryItem item)
        {
            foreach (var container in InventoryContainers)
            {
                foreach (var slot in container.Slots)
                {
                    if(slot.SlotGrid.TryPlaceItem(item)) return true;

                    foreach (var activeItem in slot.SlotGrid.ActiveItems)
                    {
                        if(activeItem.Grids.Length == 0) continue;
                        
                        foreach (var grid in activeItem.Grids)
                        {
                            if(!grid.TryPlaceItem(item)) continue;
                            
                            return true;
                        }                        
                    }                    
                }
            }

            return false;
        }


        // метод убирает из списка итемов в инвентаре определенный итем 
        public void RemoveItem(InventoryItem item)
        {
            // inventoryItems.Remove(item);
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
            // foreach (InventoryItem item in inventoryItems)
            // {
            //     //проверить на соответствие типа и так что бы item.ItemData был в model.ItemData.canBeCombined
            //     if(item.ItemData.TypeItem.HasFlag(itemType) && model.ItemData.CanBeCombined.Contains(item.ItemData) && item.Amount > 0)
            //     {
            //         return item;
            //     }
            // }

            return null;
        }

        public void RemoveAtChestGrid(InventoryItem item)
        {
            // for (int i = 0; i < inventoryItems.Count; i++)
            // {
            //     if(inventoryItems[i].ItemData == item.ItemData && inventoryItems[i].Amount == item.Amount)
            //     {
            //         inventoryItems.RemoveAt(i);
            //         return;
            //     }
            // }
        }


        public void ShowInventory()
        {
            // Debug.Log($"в инвентаре {inventoryItems.Count} экземпляров");

            // foreach (var item in inventoryItems)
            // {
            //     Debug.Log($"{item.ItemData.Title} внутри итема {item.CombinedItems.Count} предметов");

            //     foreach (var combinedItem in item.CombinedItems)
            //     {
            //         Debug.Log(item.ItemData.Title + " " + combinedItem.Value.ItemData.Title + " " + combinedItem.Value.Amount);
            //     }
            //     Debug.Log("______________________________");
            // }
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
        
        //метод одевает предмет на персонажа
        public void EquipItem(InventoryItem inventoryItem)
        {
            
        }

        //метод снимает предмет с персонажа
        public void TakeOffItem(InventoryItem inventoryItem)
        {
            
        }
    }
}
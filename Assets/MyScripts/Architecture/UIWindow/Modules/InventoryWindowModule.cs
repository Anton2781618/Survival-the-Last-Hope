using System.Collections.Generic;
using System.Linq;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UIWindow))]
    public class InventoryWindowModule : ModuleBase
    {
        [SerializeField] private GameObject windowContent;
        public List<ItemGrid> grids;

        //выбраный инвентарь
        private Inventory owner;

        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsInventory.TurnInventory, (data) => OnTurnInventory((ShowInventoryEventData)data)));
        }

        public override void UpdateMe()
        {
            
        }

        public void OnTurnInventory(ShowInventoryEventData showInventoryEventData)
        {
            SetInventoryOwner(showInventoryEventData.Inventory);

            ShowInventory(!windowContent.activeSelf);
        }

        //проверить есть ли свободное место на одной из сетке
        public ItemGrid CheckFreeSpaceForItem(InventoryItem item)
        {
            foreach (ItemGrid grid in grids)
            {
                if(grid.GridData.FindSpaceForObject(item) != null) return grid;
            }

            return null;
        }

        public void CreateAndInsertItem(InventoryItem inventoryItem, ItemGrid grid)
        {
            GlobalEventBus.Instance.Publish(EventsInventory.CreateAndInsertItem, new CreateAndInsertItemEventData
            {
                InventoryItem = inventoryItem,
                ItemGrid = grid
            });
        }

        public void DestroyInventoryItem(InventoryItem inventoryItem)
        {
            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    if(item.InventoryItem == inventoryItem)
                    {
                        owner.RemoveItem(item.InventoryItem);

                        item.DestructSelf();

                        return;
                    }
                }
            }
        }

        public void DestroyAllInventoryItem()
        {
            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    owner.RemoveItem(item.InventoryItem);

                    item.DestructSelf();
                }
            }
        }

        public void RefreshUI()
        {
            if(!gameObject.activeSelf) return;

            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    grid.GridData.CleanGridReference(item.InventoryItem);

                    item.DestructSelf();
                }
            }

            foreach (var slot in owner.Slots)
            {
                ItemGrid slotGrid = grids.FirstOrDefault(t => t.GridData.GridName == slot.SlotName);
                    //! Я закоментил это потому тогда было не понятно где брать данные сетки
                // CreateAndInsertItem(slot.SlotItem, slotGrid);
            
                // foreach (var grid in slot.SlotItem.Grids2)
                // {
                //     ItemGrid currgrid = grids.FirstOrDefault(t => t.GridData.GridName == GridData.GridInfo.BackpackGrid);

                //     foreach (var item in grid.activeItems)
                //     {
                //         if(item == null) continue;
                        
                //         CreateAndInsertItem(item, currgrid);
                //     }
                // }           
            }
        }

        public void SetInventoryOwner(Inventory newOwner)
        {
            owner = newOwner;

            grids.ForEach(t => t.GridData.owner = owner);
        }

        public void ShowInventory(bool value)
        {
            Debug.Log("Показать инвентарь " + transform.name);
            windowContent.SetActive(value);

            if(value) RefreshUI();
        }

        public void CreateRandomItem()
        {
            // inventoryManager.CreateRandomItem(grids[0]);
        }
    }
}
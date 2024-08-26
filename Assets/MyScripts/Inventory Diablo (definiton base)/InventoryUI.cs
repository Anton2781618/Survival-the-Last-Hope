using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InventoryDiablo
{
    //класс является системой управления всех ивентарей, основной функцианал инвентарей находится тут
    public class InventoryUI : MonoBehaviour, IInventoryUI
    {
        [Inject] private InventoryManager inventoryManager;
        public List<ItemGrid> grids;

        //выбраный инвентарь
        private IInventorySystem owner;

        //проверить есть ли свободное место на одной из сетке
        public ItemGrid CheckFreeSpaceForItem(InventoryItem item)
        {
            foreach (ItemGrid grid in grids)
            {
                if(grid.FindSpaceForObject(item) != null) return grid;
            }

            return null;
        }

        public void CreateAndInsertItem(InventoryItem inventoryItem, ItemGrid grid)
        {
            inventoryManager.CreateAndInsertItem(inventoryItem, grid, inventoryItem.Amount);
        }

        public void DestroyInventoryItem(InventoryItem inventoryItem)
        {
            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    if(item.InventoryItem == inventoryItem)
                    {
                        owner.InventoryHandler.Inventory.RemoveItem(item.InventoryItem);

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
                    owner.InventoryHandler.Inventory.RemoveItem(item.InventoryItem);

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
                    grid.CleanGridReference(item);

                    item.DestructSelf();
                }
            }

            foreach (InventoryItem inventoryItem in owner.InventoryHandler.Inventory.GetInventoryItems())
            {
                
                ItemGrid grid = grids.FirstOrDefault(t => t.gridName == inventoryItem.GridName);

                CreateAndInsertItem(inventoryItem, grid);
            }
            
            // foreach (ItemGrid grid in grids)
            // {
            //     foreach (UIInventoryItem item in grid.GetItems())
            //     {
            //         item.UpdateAmountText();
            //     }
            // }
        }

        public void SetInventoryOwner(IInventorySystem newOwner)
        {
            owner = newOwner;

            grids.ForEach(t => t.owner = owner);
        }

        public void ShowInventory()
        {
            Debug.Log("Показать инвентарь " + transform.name);
            gameObject.SetActive(true);

            RefreshUI();
        }

        public void CreateRandomItem()
        {
            inventoryManager.CreateRandomItem(grids[0]);
        }
            
    }
}
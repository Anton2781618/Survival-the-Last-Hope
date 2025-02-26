using System;
using System.Collections.Generic;
using System.Linq;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UIWindow))]
    public class UIInventoryWindowModule : ModuleBase
    {
        //-----------------------------------------------------------
        [Header("Блок настройки окна инвентаря")]
        [SerializeField] private GameObject windowContent;
        public List<UIItemGrid> grids;

        //-----------------------------------------------------------
        //пулл 
        [SerializeField] private UIInventoryContainer _containerPrefabs;
        private List<UIInventoryContainer> _containerList = new List<UIInventoryContainer>();
        private List<UIInventoryContainer> _containerPool = new List<UIInventoryContainer>();

        //-----------------------------------------------------------
        private Inventory _selectInventory;

        //!-----------------------------------------------------------
        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsInventory.TurnInventory, (data) => OnTurnInventory((ShowInventoryEventData)data)));
        }

        public override void UpdateMe()
        {
            
        }

        public void OnTurnInventory(ShowInventoryEventData showInventoryEventData)
        {
            if(!windowContent.activeSelf) ConstructWindow(showInventoryEventData.Inventory);

            ShowInventory(!windowContent.activeSelf);
        }

        //проверить есть ли свободное место на одной из сетке
        public UIItemGrid CheckFreeSpaceForItem(InventoryItem item)
        {
            foreach (UIItemGrid grid in grids)
            {
                if(grid.GridDataInfo.FindSpaceForObject(item) != null) return grid;
            }

            return null;
        }

        public void CreateAndInsertItem(InventoryItem inventoryItem, UIItemGrid grid)
        {
            GlobalEventBus.Instance.Publish(EventsInventory.CreateAndInsertItem, new CreateAndInsertItemEventData
            {
                InventoryItem = inventoryItem,
                ItemGrid = grid
            });
        }

        public void DestroyInventoryItem(InventoryItem inventoryItem)
        {
            foreach (UIItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    if(item.InventoryItem == inventoryItem)
                    {
                        _selectInventory.RemoveItem(item.InventoryItem);

                        item.DestructSelf();

                        return;
                    }
                }
            }
        }

        public void DestroyAllInventoryItem()
        {
            foreach (UIItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    _selectInventory.RemoveItem(item.InventoryItem);

                    item.DestructSelf();
                }
            }
        }

        public void RefreshUI()
        {
            if(!gameObject.activeSelf) return;

            foreach (UIItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    // grid.GridData.activeItems.Remove(item.InventoryItem);

                    item.DestructSelf();
                }
            }

            // foreach (var slot in _selectInventory.Slots)
            // {
            //     // ItemGrid slotGrid = grids.FirstOrDefault(t => t.GridData.GridName == slot.SlotName);
            //         //! Я закоментил это потому тогда было не понятно где брать данные сетки
            //     CreateAndInsertItem(slot.SlotItem, slotGrid);
            
            //     foreach (var grid in slot.SlotItem.Grids2)
            //     {
            //         ItemGrid currgrid = grids.FirstOrDefault(t => t.GridData.GridName == GridData.GridInfo.BackpackGrid);

            //         foreach (var item in grid.activeItems)
            //         {
            //             if(item == null) continue;
                        
            //             CreateAndInsertItem(item, currgrid);
            //         }
            //     }           
            // }
        }

        public void ConstructWindow(Inventory inventory)
        {
            _selectInventory = inventory;            

            Tool.Helper.ResetCards(_containerList, _containerPool);

            foreach (InventoryContainer container in _selectInventory.InventoryContainers)
            {
                var newContainer = Tool.Helper.GetFreeCard(_containerPrefabs, _containerPool);

                newContainer.Setup(container);

                _containerList.Add(newContainer);

                newContainer.gameObject.SetActive(true);
            }
            // grids.ForEach(t => t.GridData.OwnerInventory = _selectInventory);
        }

        public void ShowInventory(bool value)
        {
            // Debug.Log("Показать инвентарь " + transform.name);
            windowContent.SetActive(value);

            // if(value) RefreshUI();
        }

        //вызыватся снаружи кнопкой 
        public void CreateRandomItem(UIItemGrid grid)
        {
            GlobalEventBus.Instance.Publish(EventsInventory.Item_Spawned_InHand, new UIItemGridEvent
            {
                grid = grid
            });
        }


    }
        [Serializable]
        public class UIItemGridEvent : IEventData
        {    
            public UIItemGrid grid;
        }
}
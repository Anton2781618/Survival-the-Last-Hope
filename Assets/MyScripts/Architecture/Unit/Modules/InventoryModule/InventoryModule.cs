using InventoryDiablo;
using UnityEditor;
using UnityEngine;
using ModularEventArchitecture;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class InventoryModule : ModuleBase
    {
        public Inventory Inventory;

        [Tools.Button("Проверить хватает ли места для предметов")]
        private void TryPlaceItems()
        {
            foreach (var item in Inventory.Slots)
            {
                // item.TryPlaceItems();
                // foreach (var grid in item.Grids2)
                // {
                //     if(grid.ValidateItemsPosition())
                //     {
                //         Debug.Log("Места хватает");
                //     }
                //     else
                //     {
                //         Debug.Log("Места не хватает");
                //     }
                    
                // }
                
            }
        }

       
        public override void Initialize()
        {
            Entity.LocalEvents.Subscribe<EventBase>(EventsInventory.AddItem, OnAddItem);
            Entity.LocalEvents.Subscribe<EventBase>(EventsInventory.TurnInventory, OnShowInventory);
        }

        private void OnAddItem(EventBase @base)
        {
            // _inventory.AddItem(new InventoryItem());
        }

        private void OnShowInventory(EventBase eventBase)
        {
            // Публикуем глобальное событие с ссылкой на инвентарь
            GlobalEventBus.Instance.Publish(EventsInventory.TurnInventory, new ShowInventoryEventData
            {
                InventoryOwner = Entity,
                Inventory = Inventory
            });
        }

        public override void UpdateMe()
        {
            
        }
    }
}
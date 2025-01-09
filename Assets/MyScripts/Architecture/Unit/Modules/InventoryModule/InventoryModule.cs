using InventoryDiablo;
using UnityEngine;

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
                item.TryPlaceItems();
            }
        }

        public override void Initialize()
        {
            Entity.LocalEvents.Subscribe<EventBase>(EventsInventory.AddItem, OnAddItem);
            Entity.LocalEvents.Subscribe<EventBase>(EventsInventory.TurnInventory, OnShowInventory);

            TryPlaceItems();
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
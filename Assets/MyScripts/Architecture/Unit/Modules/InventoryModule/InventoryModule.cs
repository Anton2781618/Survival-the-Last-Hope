using InventoryDiablo;
using UnityEditor;
using UnityEngine;
using ModularEventArchitecture;
using System;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class InventoryModule : ModuleBase
    {
        [Serializable]
        public class weapon
        {
            public string name;
            public int damage;
        }
        public Inventory Inventory;

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
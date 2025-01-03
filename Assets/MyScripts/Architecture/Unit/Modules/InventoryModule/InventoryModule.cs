using System;
using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class InventoryModule : ModuleBase
    {
        [SerializeField] private Inventory _inventory;
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
                Inventory = _inventory
            });
        }

        public override void UpdateMe()
        {
            
        }
    }
}
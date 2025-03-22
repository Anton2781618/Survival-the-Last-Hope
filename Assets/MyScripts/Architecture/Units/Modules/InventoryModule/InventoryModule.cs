using System;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class InventoryModule : ModuleBase
    {
        //-----------------------------------------------------------
        public Inventory Inventory;

        //!-----------------------------------------------------------

        public override void Initialize()
        {
            Entity.LocalEvents.Subscribe<EventBase>(EventsInventory.TurnInventory, OnShowInventory);

            Entity.LocalEvents.Subscribe<EquipItemEventData>(EventsInventory.Equip_item_in_slot, OnSpawnObject);
            Entity.LocalEvents.Subscribe<TakeOffItemEventData>(EventsInventory.Take_off_item, OnDestroyObject);

            InitInventory();
        }

        private void OnSpawnObject(EquipItemEventData data)
        {
            Debug.Log("OnSpawnObject");
            ItemOnstreet newObjecy = Instantiate(data.InventoryItem.ItemData.Prefab, data.Parent.transform);
        }
        private void OnDestroyObject(TakeOffItemEventData data)
        {
            // data.Slot
        }

        private void InitInventory()
        {
            Inventory.Entity = Entity;
            
            foreach (var container in Inventory.InventoryContainers)
            {
                foreach (var slot in container.Slots)
                {
                    slot.Inventory = Inventory;
                }
            }
        }

        private void OnShowInventory(EventBase eventBase)
        {
            // Публикуем глобальное событие с ссылкой на инвентарь
            GlobalEventBus.Instance.Publish(EventsInventory.TurnInventory, new ShowInventoryEventData
            {
                Owner = Entity,
                InventoryOwner = Inventory
            });
        }
    }
}
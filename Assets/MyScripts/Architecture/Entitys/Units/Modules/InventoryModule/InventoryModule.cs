using System;
using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class InventoryModule : ModuleBase
    {
        //-----------------------------------------------------------
        public Inventory Inventory;

        //-----------------------------------------------------------
        //!-----------------------------------------------------------

        public override void Initialize()
        {
            Entity.LocalEvents.Subscribe<EventBase>(EventsUI.Turn_Inventory_Player, OnShowInventory);

            Entity.LocalEvents.Subscribe<EquipItemEventData>(EventsInventory.Equip_item_in_slot, OnSpawnObject);
            Entity.LocalEvents.Subscribe<TakeOffItemEventData>(EventsInventory.Take_off_item, OnDestroyObject);

            InitInventory();
        }
        
        //спавним предмет на персонаже
        private void OnSpawnObject(EquipItemEventData data)
        {
            ItemOnstreet newObject = Instantiate(data.InventoryItem.ItemData.Prefab, data.Parent.transform);

            data.Slot.ClothingItem = newObject;

            if(data.InventoryItem.ItemData.TypeItem == ItemData.ItemType.Оружие)
            {
                Entity.LocalEvents.Publish(Entitys.Player.Events.EventsAnimationWeapon.Setup_Weapon, new Entitys.Player.Events.SetupWeaponEventData
                {
                    Slot = data.Slot
                });
            }
        }
        
        private void OnDestroyObject(TakeOffItemEventData data)
        {
            Destroy(data.Slot.ClothingItem.gameObject);

            data.Slot = null; 
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
            GlobalEventBus.Instance.Publish(EventsUI.Turn_Inventory_Player, new ShowInventoryEventData
            {
                Owner = Entity,
            });
        }
    }
}
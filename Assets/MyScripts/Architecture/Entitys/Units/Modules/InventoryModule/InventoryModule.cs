using System;
using System.Collections.Generic;
using System.Linq;
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
            Entity.LocalEvents.Subscribe<EventBase>(EventsUI.Turn_Inventory_Player, OnShowInventory);

            Entity.LocalEvents.Subscribe<EquipItemEventData>(EventsInventory.Equip_item_in_slot, OnSpawnObject);
            Entity.LocalEvents.Subscribe<TakeOffItemEventData>(EventsInventory.Take_off_item, OnDestroyObject);

            Inventory.Entity = Entity;
        }
        
        //спавним предмет на персонаже
        private void OnSpawnObject(EquipItemEventData data)
        {
            ItemOnstreet newObject = Instantiate(data.InventoryItem.ItemData.Prefab, data.PlaceToSpawnClothing.transform);
            
            data.Slot.ClothingItem = newObject;

            if(data.InventoryItem.ItemData.TypeItem == ItemData.ItemType.Оружие)
            {
                Entity.LocalEvents.Publish(Entitys.Player.Events.EventsAnimationWeapon.Setup_Weapon, new Entitys.Player.Events.SetupWeaponEventData
                {
                    InventoryItem = data.InventoryItem,
                    Slot = data.Slot
                });
            }
            else
            if(data.InventoryItem.ItemData.TypeItem == ItemData.ItemType.Разгрузка)
            {
                SkinnedMeshRenderer playerSkin = data.PlaceToSpawnClothing.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer[] renderers = newObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer renderer in renderers)
                {
                    renderer.bones = playerSkin.bones;
                    renderer.rootBone = playerSkin.rootBone;
                }
            }
        }
        
        private void OnDestroyObject(TakeOffItemEventData data)
        {
            Destroy(data.Slot.ClothingItem.gameObject);

            data.Slot = null; 
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
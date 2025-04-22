using System;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    public class EventsInventory : IEventType
    {
        public int Id { get; }
        public string EventName { get; }

        private EventsInventory(string eventName)
        {
            EventName = eventName;
            // Получаем хеш-код имени события, который будет уникален
            // Добавляем префикс чтобы еще больше избежать коллизий
            Id = ("EventsInventory_" + eventName).GetHashCode();
        }

        public static IEventType AddItem => new EventsInventory("AddItem");
        // public static IEventType TurnInventory => new EventsInventory("Turn_Inventory");
        public static IEventType Select_Grid => new EventsInventory("Select_Grid");
        public static IEventType Item_Spawned_OnGrid => new EventsInventory("Item_Spawned_OnGrid");
        public static IEventType Item_Spawned_OnGround => new EventsInventory("Item_Spawned_OnGround");
        public static IEventType Item_Spawned_On_Cursor => new EventsInventory("Item_Spawned_InHand");
        public static IEventType Equip_item_in_slot => new EventsInventory("Equip_item_in_slot");
        public static IEventType Take_off_item => new EventsInventory("Take_off_item");
    }

    [Serializable]
    public class SelectGridEventData  : EventBase 
    {
        public UIItemGrid ItemGrid;
    }

    [Serializable]
    public class EquipItemEventData  : EventBase 
    {
        public InventoryItem InventoryItem;
        public GameObject PlaceToSpawnClothing;

        //слот, потому что объект с одеждой находится в слоте
        public InventorySlot Slot;
    }

    [Serializable]
    public class TakeOffItemEventData  : EventBase 
    {
        public InventorySlot Slot;
    }
    
    [Serializable]
    public class UIItemGridEvent : IEventData
    {    
        public UIItemGrid grid;
    }
}    
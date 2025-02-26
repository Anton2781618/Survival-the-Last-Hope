using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    public class EventsInventory : IEventType
    {
        private InventoryActionsEnum _type;
        private EventsInventory(InventoryActionsEnum type) => _type = type;

        public int GetEventId() => (int)_type;
        public string GetEventName() => _type.ToString();

        public static IEventType AddItem => new EventsInventory(InventoryActionsEnum.AddItem);
        public static IEventType TurnInventory => new EventsInventory(InventoryActionsEnum.Turn_Inventory);
        public static IEventType SeletGrid => new EventsInventory(InventoryActionsEnum.Selet_Grid);
        public static IEventType CreateAndInsertItem => new EventsInventory(InventoryActionsEnum.Create_And_Insert_Item);
        public static IEventType Item_Spawned_OnGround => new EventsInventory(InventoryActionsEnum.Item_Spawned_OnGround);
        public static IEventType Item_Spawned_InHand => new EventsInventory(InventoryActionsEnum.Item_Spawned_InHand);

        public enum InventoryActionsEnum
        {
            AddItem,
            RemoveItem,
            UseItem,
            DropItem,
            ShowInventory,
            Hide_Inventory,
            Turn_Inventory,
            Selet_Grid,
            Create_And_Insert_Item,
            Item_Spawned_OnGround,
            Item_Spawned_InHand,
        }
    }

    [System.Serializable]
    public class ShowInventoryEventData  : EventBase 
    {
        public GameEntity InventoryOwner { get; set; }
        public InventoryDiablo.Inventory Inventory { get; set; }
    }

    [System.Serializable]
    public class SelectGridEventData  : EventBase 
    {
        public UIItemGrid ItemGrid;
    }

    [System.Serializable]
    public class CreateAndInsertItemEventData  : EventBase 
    {
        public InventoryItem InventoryItem;
        public UIItemGrid ItemGrid;
    }
}    
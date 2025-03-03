using InventoryDiablo;

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
        public static IEventType TurnInventory => new EventsInventory("Turn_Inventory");
        public static IEventType SeletGrid => new EventsInventory("Selet_Grid");
        public static IEventType CreateAndInsertItem => new EventsInventory("Create_And_Insert_Item");
        public static IEventType Item_Spawned_OnGround => new EventsInventory("Item_Spawned_OnGround");
        public static IEventType Item_Spawned_InHand => new EventsInventory("Item_Spawned_InHand");
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
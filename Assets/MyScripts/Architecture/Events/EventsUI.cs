using UnityEngine;

namespace ModularEventArchitecture
{
    public class EventsUI : IEventType
    {
        public int Id { get; }
        public string EventName { get; }

        private EventsUI(string eventName)
        {
            EventName = eventName;
            // Получаем хеш-код имени события, который будет уникален
            // Добавляем префикс чтобы еще больше избежать коллизий
            Id = ("EventsUI_" + eventName).GetHashCode();
        }

        public static readonly IEventType Show_Text = new EventsUI("Show_Text");
        public static IEventType Turn_Inventory_Player => new EventsUI("Turn_Inventory_Player");
        public static IEventType Turn_Inventory_Unit_And_Player => new EventsUI("Turn_Inventory_Unit_And_Player");
        public static IEventType Turn_Inventory_InteractableObject => new EventsUI("Turn_Inventory_InteractableObject");
        public static IEventType Show_window => new EventsUI("Show_window");
    }
    
    [System.Serializable]
    public class EventShowText : EventBase
    {
        public string Text;
        public Vector3 Position;
    }

    [System.Serializable]
    public class ShowInventoryEventData  : EventBase 
    {
        public GameEntity Owner { get; set; }
    }
    [System.Serializable]
    public class ShowInventory2EventData  : EventBase 
    {
        public GameEntity Player { get; set; }
        public GameEntity Unit { get; set; }
    }
}
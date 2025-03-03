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
    }
    
    [System.Serializable]
    public class EventShowText : EventBase
    {
        public string Text;
        public Vector3 Position;
    }
}
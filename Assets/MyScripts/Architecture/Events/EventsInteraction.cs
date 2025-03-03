using System;
using UnityEngine;

namespace ModularEventArchitecture
{
    public class EventsInteraction : IEventType
    {
        public int Id { get; }
        public string EventName { get; }

        private EventsInteraction(string eventName)
        {
            EventName = eventName;
            // Получаем хеш-код имени события, который будет уникален
            // Добавляем префикс чтобы еще больше избежать коллизий
            Id = ("EventsInteraction_" + eventName).GetHashCode();
        }

        public static readonly IEventType Try_Interact = new EventsInteraction("Try_Interact");
        public static readonly IEventType Sent_Interact_Unit = new EventsInteraction("Sent_Interact_Unit");
    }

    [Serializable]
    public class TryInteractEvent : IEventData 
    {
        public GameObject InteractUnit;
    }
}
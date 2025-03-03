
using InventoryDiablo;


namespace ModularEventArchitecture
{
    public class EventsSpawner : IEventType
    {
        public int Id { get; }
        public string EventName { get; }

        private EventsSpawner(string eventName)
        {
            EventName = eventName;
            // Получаем хеш-код имени события, который будет уникален
            // Добавляем префикс чтобы еще больше избежать коллизий
            Id = ("EventsSpawner_" + eventName).GetHashCode();
        }

        // Статические свойства для доступа к событиям
        public static IEventType SpawnUnitOnStreet => new EventsSpawner("SpawnUnitOnStreet");
        public static IEventType SpawnWeaponOnUnit => new EventsSpawner("SpawnWeaponOnUnit");
    }
    
    [System.Serializable]
    public class EventDataUnit : EventBase
    {
        public InventoryItem Item;
    }
}
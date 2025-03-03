using ModularEventArchitecture;

namespace Entitys.Player.Events
{
    public class EventsWeapon : IEventType
    {
        public int Id { get; }
        public string EventName { get; }

        private EventsWeapon(string eventName)
        {
            EventName = eventName;
            // Получаем хеш-код имени события, который будет уникален
            // Добавляем префикс чтобы еще больше избежать коллизий
            Id = ("EventsWeapon_" + eventName).GetHashCode();
        }

        public static IEventType StartFire => new EventsWeapon("StartFire");
        public static IEventType StopFire => new EventsWeapon("StopFire");
        public static IEventType Reload => new EventsWeapon("Reload");
        public static IEventType ChangeAmmo => new EventsWeapon("ChangeAmmo");
        public static IEventType ChangeWeapon => new EventsWeapon("ChangeWeapon");
    }        
}

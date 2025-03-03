using ModularEventArchitecture;

namespace Entitys.Player.Events
{
    public class EventsAnimationWeapon : IEventType
    {
        public int Id { get; }
        public string EventName { get; }

        private EventsAnimationWeapon(string eventName)
        {
            EventName = eventName;
            // Получаем хеш-код имени события, который будет уникален
            // Добавляем префикс чтобы еще больше избежать коллизий
            Id = ("EventsAnimationWeapon_" + eventName).GetHashCode();
        }

        public static IEventType HolsterWeaponStart => new EventsAnimationWeapon("HolsterWeaponStart");
        public static IEventType DrawWeaponStart => new EventsAnimationWeapon("DrawWeaponStart");
        public static IEventType AimWeapon => new EventsAnimationWeapon("AimWeapon");
        public static IEventType OffAim => new EventsAnimationWeapon("OffAim");
        public static IEventType Fire => new EventsAnimationWeapon("Fire");
        public static IEventType StopFire => new EventsAnimationWeapon("Fire");
    }
}

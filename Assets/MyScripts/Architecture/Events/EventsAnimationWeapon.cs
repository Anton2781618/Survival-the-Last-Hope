using System;
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

        public static IEventType Holster_Weapon_Start => new EventsAnimationWeapon("Holster_Weapon_Start");
        public static IEventType Draw_Weapon_Start => new EventsAnimationWeapon("Draw_Weapon_Start");
        public static IEventType AimWeapon => new EventsAnimationWeapon("AimWeapon");
        public static IEventType Off_Aim => new EventsAnimationWeapon("Off_Aim");
        public static IEventType Fire => new EventsAnimationWeapon("Fire");
        public static IEventType Stop_Fire => new EventsAnimationWeapon("Stop_Fire");
        public static IEventType Setup_Weapon => new EventsAnimationWeapon("Setup_Weapon");
    }

    [Serializable]
    public class SetupWeaponEventData  : EventBase 
    {
        public InventorySlot Slot;
    }
}

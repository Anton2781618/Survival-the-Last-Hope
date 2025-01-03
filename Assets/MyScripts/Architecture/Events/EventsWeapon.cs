using System.Collections;
using System.Collections.Generic;
using ModularEventArchitecture;
using UnityEngine;

namespace Entitys.Player.Events
{
    public class EventsWeapon : IEventType
    {
        private WeaponActionsEnum _type;

        private EventsWeapon(WeaponActionsEnum type)
        {
            _type = type;
        }

        public int GetEventId() => (int)_type;
        public string GetEventName() => _type.ToString();

        public static IEventType StartFire => new EventsWeapon(WeaponActionsEnum.StartFire);
        public static IEventType StopFire => new EventsWeapon(WeaponActionsEnum.StopFire);
        public static IEventType Reload => new EventsWeapon(WeaponActionsEnum.Reload);
        public static IEventType ChangeAmmo => new EventsWeapon(WeaponActionsEnum.ChangeAmmo);
        public static IEventType ChangeWeapon => new EventsWeapon(WeaponActionsEnum.ChangeWeapon);

        public enum WeaponActionsEnum
        {
            StartFire,
            StopFire,
            Reload,
            ChangeAmmo,
            ChangeWeapon
        }
    }        
}

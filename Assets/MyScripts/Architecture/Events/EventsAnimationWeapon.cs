using System.Collections;
using System.Collections.Generic;
using ModularEventArchitecture;
using UnityEngine;

namespace Entitys.Player.Events
{
    public class EventsAnimationWeapon : IEventType
    {
        private AnimationsWeaponActionsEnum _type;

        private EventsAnimationWeapon(AnimationsWeaponActionsEnum type)
        {
            _type = type;
        }

        public int GetEventId() => (int)_type;
        public string GetEventName() => _type.ToString();

        public static IEventType HolsterWeaponStart => new EventsAnimationWeapon(AnimationsWeaponActionsEnum.HolsterWeaponStart);
        public static IEventType DrawWeaponStart => new EventsAnimationWeapon(AnimationsWeaponActionsEnum.DrawWeaponStart);
        public static IEventType AimWeapon => new EventsAnimationWeapon(AnimationsWeaponActionsEnum.AimWeapon);
        public static IEventType OffAim => new EventsAnimationWeapon(AnimationsWeaponActionsEnum.OffAim);
        public static IEventType Fire => new EventsAnimationWeapon(AnimationsWeaponActionsEnum.Fire);
        public static IEventType StopFire => new EventsAnimationWeapon(AnimationsWeaponActionsEnum.StopFire);

        public enum AnimationsWeaponActionsEnum
        {
            HolsterWeaponStart,
            DrawWeaponStart,
            AimWeapon,
            OffAim,
            Fire,
            StopFire,
        }
    }
}

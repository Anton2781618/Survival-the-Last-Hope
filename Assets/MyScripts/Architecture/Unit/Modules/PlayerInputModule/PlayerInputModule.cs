using System.Collections;
using System.Collections.Generic;
using Entitys.Player.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    public class PlayerInputModule : ModuleBase
    {
        public override void Initialize()
        {
            
        }

        public override void UpdateMe()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.Fire, new EventBase());
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.StopFire, new EventBase());
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.AimWeapon, new EventBase());
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.OffAim, new EventBase());
            }

            if(Input.GetKeyDown(KeyCode.F))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.DrawWeaponStart, new EventBase());
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Entity.LocalEvents.Publish(EventsInventory.TurnInventory, new EventBase());
            }
        }
    }
}

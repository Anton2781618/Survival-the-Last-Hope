using Entitys.Player.Events;
using UnityEngine;

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

            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Try_Interact !!!!!!!!!!");
                Entity.LocalEvents.Publish(EventsInteraction.Try_Interact, new EventBase());
            }
        }
    }
}

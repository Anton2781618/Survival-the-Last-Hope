using Entitys.Player.Events;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(Player))]
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
                Entity.LocalEvents.Publish(EventsAnimationWeapon.Stop_Fire, new EventBase());
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.AimWeapon, new EventBase());
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.Off_Aim, new EventBase());
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.Reload, new EventBase());
            }

            if(Input.GetKeyDown(KeyCode.F))
            {
                Entity.LocalEvents.Publish(EventsAnimationWeapon.Draw_Weapon_Start, new EventBase());
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Entity.LocalEvents.Publish(EventsUI.Turn_Inventory_Player, new EventBase());
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                Entity.LocalEvents.Publish(EventsInteraction.Try_Interact, new EventBase());
            }
        }
    }
}

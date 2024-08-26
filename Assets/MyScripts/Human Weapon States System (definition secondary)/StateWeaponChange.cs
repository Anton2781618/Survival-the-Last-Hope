using UnityEngine;
using Weapons;

namespace States
{
    public class StateWeaponChange : WeaponState
    {
        private WeaponBase newWeapon;

        public void SetNewWeapon(WeaponBase newWeapon)
        {
            this.newWeapon = newWeapon;
        }

        public override void Execute()
        {
            if(!newWeapon)
            {
                Debug.Log("newWeapon == null");
                
                stateService.inputSystem.weaponChange = false;

                stateService.TransitionTo(stateService.stateWeaponOff);
                
                return;
            } 

            stateService.CurrentWeapon = newWeapon;

            stateService.inputSystem.weaponOn = true;

            stateService.inputSystem.weaponChange = false;

            stateService.playerModel.rigTargetRight.localPosition = stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? 
            ConvertePoint(stateService.playerModel.PistolHolster.position) : stateService.playerModel.RifleHolster.localPosition;

            stateService.playerModel.rigTargetRight.rotation = stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? 
            stateService.playerModel.PistolHolster.rotation : stateService.playerModel.RifleHolster.rotation;

            stateService.TransitionTo(stateService.stateHeandOnWeapon);

            Debug.Log("Смена оружия на " + newWeapon);
        }
    }
}
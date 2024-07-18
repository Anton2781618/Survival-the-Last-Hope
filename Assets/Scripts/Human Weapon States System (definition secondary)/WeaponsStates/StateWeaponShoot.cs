using UnityEngine;

namespace States
{
    internal class StateWeaponShoot : WeaponState
    {
        public override void Execute()
        {
            if (!stateService.inputs.shoot)
            {
                stateService.TransitionTo(stateService.stateWeaponAim);
            }

            Shoot();
        }

        private void Shoot() => stateService.CurrentWeapon.Fire();
    }
}

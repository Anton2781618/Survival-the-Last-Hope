using UnityEngine;

namespace States
{
    public class StateHeandOnWeapon : WeaponState
    {
        public override void Execute()
        {
            if(stateService.inputs.weaponOn && stateComplete)
            {
                stateService.stateWeaponOn.TransitionForward = true;

                stateService.TransitionTo(stateService.stateWeaponOn);

                return;
            }

            SetWeaponInHend();
        }

        public override void MoveHendsFromPoint() => Debug.LogError("Это вызываться не должно");

        private void SetWeaponInHend()
        {
            if(SetWeaponsAtReady())
            {
                stateService.CurrentWeapon.DrawWeapon(stateService.playerModel.hend);

                stateComplete = true;
            }
        }

        //персонаж с оружием в руках
        private bool SetWeaponsAtReady()
        {
            stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5);

            return stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 1, Time.deltaTime * 5) > 0.9;
        }
    }
}
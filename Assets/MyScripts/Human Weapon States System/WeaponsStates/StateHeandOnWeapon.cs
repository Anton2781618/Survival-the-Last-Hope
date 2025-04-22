using UnityEngine;

namespace States
{
    //увеличивает вес рига на слое aim
    public class StateHeandOnWeapon : WeaponState
    {
        public override void Execute()
        {
            SetWeaponInHend();
        }

        public override void MoveHendsFromPoint() => Debug.LogError("Это вызываться не должно");

        private void SetWeaponInHend()
        {
            if(SetWeaponsAtReady())
            {
                CurrentWeapon.DrawWeapon(UnitModel.hend);

                IsComplete = true;
            }
        }

        //персонаж с оружием в руках
        private bool SetWeaponsAtReady()
        {
            UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5);

            return UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 1, Time.deltaTime * 5) > 0.9;
        }
    }
}
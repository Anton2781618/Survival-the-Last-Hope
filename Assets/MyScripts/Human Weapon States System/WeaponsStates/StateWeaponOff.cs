using UnityEngine;
using static ModularEventArchitecture.WeaponBaseModule;

namespace States
{
    //перемещает риг в кабуру по кривой и вращает его в 
    public class StateWeaponOff : WeaponState
    {
        private Transform toPosition;
        public override void Init()
        {
            base.Init();

            FromRotationRight = UnitModel.rigTargetRight.rotation;

            lerpRatio = 1;

            stateTimer = 0.4f;

            LerpTime = 0.4f;

            if(CurrentWeapon)
            {
                toPosition = CurrentWeapon.TypeWeapon == WeaponType.Pistol ? UnitModel.PistolHolster : UnitModel.RifleHolster;
            }
            else
            {
                toPosition = UnitModel.PistolHolster;
            }
        }

        public override void Execute()
        {         
            MoveHendsFromPoint();

            WeaponOff();
        }

        private new void UpdateLerpRatio()
        {
            stateTimer -= Time.deltaTime;

            if(stateTimer < 0) stateTimer = 0;

            lerpRatio = stateTimer / LerpTime;
        }

        public override void MoveHendsFromPoint()
        {
            Vector3 positionOffset = CurrentWeapon ? CurrentWeapon.WeaponModel.LerpCurve.Evaluate(lerpRatio) * CurrentWeapon.WeaponModel.LerpOffset : Vector3.zero;

            Transform rigTargetRight = UnitModel.rigTargetRight.transform;

            rigTargetRight.localPosition = Vector3.Lerp(ConvertePoint(toPosition.position), FromPointRight, lerpRatio) + positionOffset;

            rigTargetRight.rotation = Quaternion.Lerp(toPosition.rotation, FromRotationRight, lerpRatio);

            //убераем вес левой руки
            UnitModel.SetWeightLeftHand(lerpRatio);

            UpdateLerpRatio();
        }

        // плавно двигать rigBuilder.layers[(int)RigLayers.NoAim] к точке points[0], затем двигать плавно к точке points[1], затем выключить WeaponsAtReady()
        private void WeaponOff()
        {
            if(lerpRatio <= 0)
            {
                CurrentWeapon?.HolsterWeapon(CurrentWeapon.TypeWeapon == WeaponType.Pistol ? UnitModel.PistolHolster : UnitModel.RifleHolster);

                SetNoWeapon();
            }
        }

        //убрать вес со слоя так что юы персонаж был без оружия
        private void SetNoWeapon()
        {
            if(UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, -0.1f, Time.deltaTime * 5) <= 0 && 
            UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, -0.1f, Time.deltaTime * 5) <= 0) IsComplete = true;
        }
    }
}
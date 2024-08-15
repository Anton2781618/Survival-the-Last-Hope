using UnityEngine;
namespace States
{
    public class StateWeaponOff : WeaponState
    {
        public override void Init()
        {
            base.Init();

            FromRotation = stateService.playerModel.rigTargetRight.rotation;

            lerpRatio = 1;

            stateTimer = 0.4f;

            // if(stateService.CurrentWeapon)
            // {
            //     FromPoint = /* stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ?  */stateService.playerModel.PistolHolster.localPosition /* : stateService.playerModel.RifleHolster.localPosition */;
            // }
        }

        public override void Execute()
        {

            if(!stateService.inputSystem.weaponOn && stateService.inputSystem.weaponChange && IsComplete)
            {
                stateService.TransitionTo(stateService.stateWeaponChange);
            }
            else
            if(!stateService.inputSystem.weaponOn && IsComplete) 
            {
                return;
            }
            else
            if(stateService.inputSystem.weaponOn) 
            {
                stateService.TransitionTo(stateService.stateHeandOnWeapon);
            }

            MoveHendsFromPoint();

            WeaponOff();
        }

        private new void UpdateLerpRatio()
        {
            stateTimer -= Time.deltaTime;

            if(stateTimer < 0) stateTimer = 0;

            lerpRatio = stateTimer / stateService.LerpTime;
        }

        public override void MoveHendsFromPoint()
        {
            Vector3 positionOffset = stateService.playerModel.lerpCurve.Evaluate(lerpRatio) * stateService.playerModel.lerpOffset;

            Transform rightHend = stateService.playerModel.rigTargetRight.transform;
            
            Transform toPosition = stateService.playerModel.PistolHolster;
            
            if(stateService.CurrentWeapon)
            {
                // FromPoint = stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? stateService.playerModel.PistolHolster.localPosition : stateService.playerModel.RifleHolster.localPosition;
                // toPosition = stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? stateService.playerModel.PistolHolster : stateService.playerModel.RifleHolster;
            }

            rightHend.localPosition = Vector3.Lerp(ConvertePoint(toPosition.position), FromPoint, lerpRatio) + positionOffset;

            rightHend.rotation = Quaternion.Lerp(toPosition.rotation, FromRotation, lerpRatio);

            //убераем вес левой руки
            stateService.playerModel.SetWeightLeftHand(lerpRatio);

            UpdateLerpRatio();
        }

        public Vector3 ConvertePoint(Vector3 point) => stateService.playerModel.objectForLocalSpace.worldToLocalMatrix.MultiplyPoint(point);


        // плавно двигать rigBuilder.layers[(int)RigLayers.NoAim] к точке points[0], затем двигать плавно к точке points[1], затем выключить WeaponsAtReady()
        private void WeaponOff()
        {
            if(lerpRatio <= 0)
            {
                stateService.CurrentWeapon?.HolsterWeapon(stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? stateService.playerModel.PistolHolster : stateService.playerModel.RifleHolster);

                SetNoWeapon();
            }
        }

        //убрать вес со слоя так что юы персонаж был без оружия
        private void SetNoWeapon()
        {
            if(stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, -0.1f, Time.deltaTime * 5) <= 0 && 
            stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, -0.1f, Time.deltaTime * 5) <= 0) IsComplete = true;
        }
    }
}
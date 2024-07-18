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
        }

        public override void Execute()
        {
            if(!stateService.inputs.weaponOn && stateComplete) return;

            if(stateService.inputs.weaponOn) 
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
            
            Transform ToPosition = stateService.playerModel.ToPosition;

            rightHend.localPosition = Vector3.Lerp(ConvertePoint(ToPosition.position), FromPoint, lerpRatio) + positionOffset;

            rightHend.rotation = Quaternion.Lerp(ToPosition.rotation, FromRotation, lerpRatio);

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
                stateService.CurrentWeapon?.HolsterWeapon(stateService.playerModel.ToPosition);

                SetNoWeapon();
            }
        }

        //убрать вес со слоя так что юы персонаж был без оружия
        private void SetNoWeapon()
        {
            if(stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 0, Time.deltaTime * 5) == 0 && 
            stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5) == 0) stateComplete = true;
        }
    }
}
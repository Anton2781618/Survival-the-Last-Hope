// using ModestTree;
using UnityEngine;

namespace States
{
    public class StateWeaponAim : WeaponState
    {
        public override void Init()
        {
            base.Init();

            lerpRatio = 0;

            stateTimer = 0;

            ToPoint = stateService.CurrentWeapon.GetModel().WeaponStates[1].RightHendPosition;
        }

        public override void Execute()
        {
            if(stateService.inputs.reload)
            {
                stateService.TransitionTo(stateService.stateWeaponReload);
            }
            else
            if(!stateService.inputs.aim)
            {
                stateService.stateWeaponOn.TransitionForward = false;

                stateService.TransitionTo(stateService.stateWeaponOn);
            }
            else
            if(stateService.inputs.shoot && stateComplete)
            {
                stateService.TransitionTo(stateService.stateWeaponShoot);
            }

            if(!stateService.playerModel.animator.GetBool("Aim")) stateService.playerModel.animator.SetBool("Aim", true);
            
            MoveHendsFromPoint();

            if(SetWeaponsAtReady() && lerpRatio >=1) stateComplete = true;
            
        }

        //прицеливание пистолета
        public override void MoveHendsFromPoint()
        {
            if(lerpRatio >= 1) return;

            stateService.playerModel.rigTargetRight.transform.localPosition = Vector3.Lerp(FromPoint, ToPoint, lerpRatio);

            stateService.playerModel.rigTargetLeft.transform.localPosition = stateService.CurrentWeapon.GetModel().WeaponStates[1].LeftHendPosition;

            UpdateLerpRatio();
        }

        private bool SetWeaponsAtReady()
        {
            bool a = stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 0, Time.deltaTime * 10) <= 0.1f;

            bool b = stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 1, Time.deltaTime * 10) > 0.9f;

            return a && b; 
        }
    }
}
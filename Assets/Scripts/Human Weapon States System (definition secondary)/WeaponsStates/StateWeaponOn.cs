using UnityEngine;

namespace States
{
    public class StateWeaponOn : WeaponState
    {
        public override void Init()
        {
            base.Init();

            ToPoint = stateService.CurrentWeapon.GetModel().WeaponStates[0].RightHendPosition;
            
            ToRotation = stateService.CurrentWeapon.GetModel().WeaponStates[0].RightHendRotation;

            lerpCurve = stateService.playerModel.lerpCurve;

            lerpOffset = stateService.playerModel.lerpOffset;
        }
        
        public override void Execute()
        {
            if(!stateService.inputs.weaponOn && stateComplete || stateService.CurrentWeapon == null)
            {
                stateService.TransitionTo(stateService.stateWeaponOff);
            }
            else
            if(stateService.inputs.reload)
            {
                stateService.TransitionTo(stateService.stateWeaponReload);
            }
            else
            if(stateService.inputs.aim && lerpRatio > 0.9f) 
            {
                stateService.TransitionTo(stateService.stateWeaponAim);
            }
            else
            if(stateService.inputs.weaponOn && stateComplete) return;
            
            if(stateService.playerModel.animator.GetBool("Aim")) stateService.playerModel.animator.SetBool("Aim", false);


            MoveHendsFromPoint();

            UpdateLerpRatio();

            if(SetWeaponsAtReady() && lerpRatio >=1) stateComplete = true;
        }

        private bool SetWeaponsAtReady()
        {
            // stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, lerpRatio);
            stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5);

            // return stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 1, lerpRatio) > 0.95f;
            return stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 1, Time.deltaTime * 5) > 0.95f;
        }
    }
}


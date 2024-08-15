using UnityEngine;

namespace States
{
    public class StateWeaponOn : WeaponState
    {
        public override void Init()
        {
            base.Init();
            
            Debug.Log("Init " + stateService.CurrentWeapon.GetModel().WeaponStates[0].RightHendPosition);
            ToPoint = stateService.CurrentWeapon.GetModel().WeaponStates[0].RightHendPosition;
            
            // stateService.playerModel.rigTargetRight.localPosition = 
            // stateService.CurrentWeapon.TypeWeapon == Weapons.WeaponBase.WeaponType.Pistol ? stateService.playerModel.PistolHolster.localPosition : stateService.playerModel.RifleHolster.localPosition;
            
            // FromPoint = stateService.playerModel.rigTargetRight.localPosition;
            
            ToRotation = stateService.CurrentWeapon.GetModel().WeaponStates[0].RightHendRotation;

            lerpCurve = stateService.playerModel.lerpCurve;

            lerpOffset = stateService.playerModel.lerpOffset;
        }
        
        public override void Execute()
        {
            if(!stateService.inputSystem.weaponOn && IsComplete || stateService.CurrentWeapon == null)
            {
                stateService.TransitionTo(stateService.stateWeaponOff);
            }
            else
            if(stateService.inputSystem.reload)
            {
                stateService.TransitionTo(stateService.stateWeaponReload);
            }
            else
            if(stateService.inputSystem.aim && lerpRatio > 0.9f) 
            {
                stateService.TransitionTo(stateService.stateWeaponAim);
            }
            else
            if(stateService.inputSystem.weaponOn && IsComplete) return;
            
            if(stateService.playerModel.animator.GetBool("Aim")) stateService.playerModel.animator.SetBool("Aim", false);


            MoveHendsFromPoint();

            UpdateLerpRatio();

            if(SetWeaponsAtReady() && lerpRatio >=1) IsComplete = true;
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


using UnityEngine;

namespace States
{
    //увеличивает вес рига на слое aim
    public class StateWeaponOn : WeaponState
    {
        public override void Init()
        {
            base.Init();

            lerpRatio = 0;

            stateTimer = 0;

            LerpTime = 0.4f;
        }
        
        public override void Execute()
        {
            // if(!stateService.inputSystem.weaponOn && IsComplete || stateService.CurrentWeapon == null)
            // {
            //     stateService.TransitionTo(stateService.stateWeaponOff);
            // }
            // else
            // if(stateService.inputSystem.reload)
            // {
            //     stateService.TransitionTo(stateService.stateWeaponReload);
            // }
            // else
            // if(stateService.inputSystem.aim && lerpRatio > 0.9f) 
            // {
            //     stateService.TransitionTo(stateService.stateWeaponAim);
            // }
            // else
            // if(stateService.inputSystem.weaponOn && IsComplete) return;
            
            // if(stateService.playerModel.animator.GetBool("Aim")) stateService.playerModel.animator.SetBool("Aim", false);
            if(UnitModel.animator.GetBool("Aim")) UnitModel.animator.SetBool("Aim", false);

            MoveHendsFromPoint();

            if(SetWeaponsAtReady() && lerpRatio >= 0.95f) IsComplete = true;
        }

        private bool SetWeaponsAtReady()
        {
            UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5);

            return UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 1, Time.deltaTime * 5) > 0.99f;
        }
    }
}
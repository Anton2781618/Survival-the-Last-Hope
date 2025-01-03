// using ModestTree;
using UnityEngine;

namespace States
{
    //
    public class StateWeaponAim : WeaponState
    {
        public override void Init()
        {
            base.Init();

            lerpRatio = 0;

            stateTimer = 0;
        }

        public override void Execute()
        {
            // if(stateService.inputSystem.reload)
            // {
            //     stateService.TransitionTo(stateService.stateWeaponReload);
            // }
            // else
            // if(!stateService.inputSystem.aim)
            // {
            //     stateService.stateWeaponOn.TransitionForward = false;

            //     stateService.TransitionTo(stateService.stateWeaponOn);
            // }
            // else
            // if(stateService.inputSystem.shoot && IsComplete)
            // {
            //     stateService.TransitionTo(stateService.stateWeaponShoot);
            // }

            if(!UnitModel.animator.GetBool("Aim")) UnitModel.animator.SetBool("Aim", true);
            
            MoveHendsFromPoint();

            if(SetWeaponsAtReady() && lerpRatio >=1) IsComplete = true;
            
        }

        private bool SetWeaponsAtReady()
        {
            bool a = UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 0, Time.deltaTime * 10) <= 0.1f;

            bool b = UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 1, Time.deltaTime * 10) > 0.9f;

            return a && b; 
        }
    }
}
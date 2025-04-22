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
using InventoryDiablo;
using UnityEngine;

namespace States
{
    internal class StateWeaponReload : WeaponState
    {
        public override void Execute()
        {
            Reload();

            /* if (!stateService.CurrentWeapon.IsReloadingNow())
            {
                needMoveHeand = true;

                stateService.TransitionTo(stateService.stateWeaponOn);
                
                stateService.CurrentWeapon.InventoryItem.OnItemsChanged?.Invoke();
                Debug.Log("Reload complete");
            } */
        }

        private void Reload()
        {
            MoveHendsFromPoint();
            // if (stateService.inputSystem.reload)
            // if (needMoveHeand)
            // {
                // IsComplete = false;

                // needMoveHeand = false;

                // UnitModel.animator.SetTrigger("Reload");

                // stateService.inputSystem.reload = false;

                // stateService.CurrentWeapon.StartCoroutine(stateService.CurrentWeapon.Reload(stateService.owner));
            // }

            // SetNoWeapon();
            if(lerpRatio >=1) IsComplete = true;
        }

        //убрать вес со слоя так что юы персонаж был без оружия
        private void SetNoWeapon()
        {
            if(UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 0, Time.deltaTime * 5) == 0 && 
            UnitModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5) == 0) IsComplete = true;
        }
    }
}

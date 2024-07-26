using InventoryDiablo;
using UnityEngine;

namespace States
{
    internal class StateWeaponReload : WeaponState
    {
        public override void Execute()
        {
            Reload();

            if (!stateService.CurrentWeapon.IsReloadingNow())
            {
                needMoveHeand = true;

                stateService.stateWeaponOn.TransitionForward = false;

                stateService.StateComplete();

                stateService.TransitionTo(stateService.stateWeaponOn);
                
                stateService.CurrentWeapon.InventoryItem.OnItemsChanged?.Invoke();
                Debug.Log("Reload complete");
            }
        }

        private void Reload()
        {
            if (stateService.inputs.reload)
            {
                stateComplete = false;

                needMoveHeand = false;

                stateService.playerModel.animator.SetTrigger("Reload");

                stateService.inputs.reload = false;

                stateService.CurrentWeapon.StartCoroutine(stateService.CurrentWeapon.Reload(stateService.owner));
            }

            SetNoWeapon();
        }

        //убрать вес со слоя так что юы персонаж был без оружия
        private void SetNoWeapon()
        {
            if(stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.Aim, 0, Time.deltaTime * 5) == 0 && 
            stateService.playerModel.SetWeightRigBuilderlayer((int)Units.HumanModel.RigLayers.NoAim, 0, Time.deltaTime * 5) == 0) stateComplete = true;
        }
    }
}

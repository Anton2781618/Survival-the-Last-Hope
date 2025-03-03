using UnityEngine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using Zenject;
using States;
using InventoryDiablo;
using Units;

namespace Tool
{
    public class PLayerInstaller : MonoInstaller
    {
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private Transform debugTarget;
        [SerializeField] private SettingsRaycaster settingsRaycaster;
        // [SerializeField] private InventoryUI inventoryUI; 
        [SerializeField] private InventoryHandler inventoryHandler; 

        public override void InstallBindings()
        {

            Container.Bind<RigBuilder>().FromComponentSibling(); // найти в компонентах найденного объекта и вставить к нему

            Container.Bind<IWeaponStates>().To<HumanWeaponStateManager>().AsSingle();

            Container.Bind<IRaycastHandler>().To<RaycastService>().AsSingle().WithArguments(debugTarget, settingsRaycaster);


            
            // Container.Bind<IInventory>().To<Inventory>().AsTransient();
            
            // Container.Bind<IInventoryUI>().To<InventoryUI>().FromInstance(inventoryUI).AsSingle();
            
            Container.Bind<InventoryHandler>().FromInstance(inventoryHandler).AsSingle();

            Container.Bind<CharacterController>().FromComponentSibling().AsSingle();
        }
    }
}
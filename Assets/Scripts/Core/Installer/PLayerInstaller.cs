using UnityEngine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using Zenject;
using States;
using InventoryDiablo;
using Units;

namespace MyProject
{
    public class PLayerInstaller : MonoInstaller
    {
        [SerializeField] private Player player;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private Transform debugTarget;
        [SerializeField] private SettingsRaycaster settingsRaycaster;
        [SerializeField] private InventoryUI inventoryUI; 

        public override void InstallBindings()
        {
            Container.Bind<IMuveHandler>().To<PLayerMoveSystem>().AsSingle().WithArguments(starterAssetsInputs, player.humanModel.animator); // забиндить в поле вместе с аргументами

            Container.Bind<RigBuilder>().FromComponentSibling(); // найти в компонентах найденного объекта и вставить к нему

            Container.Bind<IWeaponStates>().To<HumanWeaponStateManager>().AsSingle();

            Container.Bind<IRaycastHandler>().To<RaycastService>().AsSingle().WithArguments(debugTarget, settingsRaycaster);

            Container.Bind<HumanModel>().FromInstance(player.humanModel).AsSingle();

            
            Container.Bind<IInventory>().To<Inventory>().AsTransient();
            
            Container.Bind<IInventoryUI>().To<InventoryUI>().FromInstance(inventoryUI).AsSingle();

            Container.Bind<IInventoryController>().To<InventoryController>().AsTransient();
        }
    }
}
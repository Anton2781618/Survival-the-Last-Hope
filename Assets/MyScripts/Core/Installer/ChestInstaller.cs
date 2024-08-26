using InventoryDiablo;
using UnityEngine;
using Zenject;

public class ChestInstaller : MonoInstaller
{
    [SerializeField] private InventoryUI inventoryUI; 

    [SerializeField] private InventoryHandler inventoryHandler;
    public override void InstallBindings()
    {
        Container.Bind<IInventoryUI>().To<InventoryUI>().FromInstance(inventoryUI).AsSingle();

        Container.Bind<InventoryHandler>().FromInstance(inventoryHandler).AsSingle();

        Container.Bind<Collider>().FromComponentSibling().AsSingle();
    }
}
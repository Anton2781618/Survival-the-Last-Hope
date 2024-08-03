using UnityEngine;
using Zenject;
using StarterAssets;
using MyProject;
using UnityEngine.EventSystems;
using InventoryDiablo;


public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private Spawner spawner;
    [SerializeField] private InventoryManager inventoryManager; 
    [SerializeField] private Canvas canvas; 

    public override void InstallBindings()
    {
        Container.Bind<StarterAssetsInputs>().FromInstance(starterAssetsInputs).AsSingle();

        Container.Bind<InventoryManager>().FromInstance(inventoryManager).AsSingle();

        Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
        
        Helper.spawner = spawner;
    }
}

using UnityEngine;
using Zenject;
using StarterAssets;
using MyProject;
using UnityEngine.EventSystems;
using InventoryDiablo;


public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private Canvas canvas; 

    public override void InstallBindings()
    {
        Container.Bind<StarterAssetsInputs>().FromComponentInHierarchy().AsSingle();

        Container.Bind<InventoryManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
        
        Helper.spawner = spawner;
    }
}

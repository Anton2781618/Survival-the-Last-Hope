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
    [SerializeField] private GameDataBase gameDataBase; 

    public override void InstallBindings()
    {
        Container.Bind<StarterAssetsInputs>().FromComponentInHierarchy().AsSingle();

        Container.Bind<InventoryManager>().FromComponentInHierarchy().AsSingle();
        
        

        Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
        
        Helper.Spawner = spawner;

        Helper.GameDataBase = gameDataBase;
    }
}

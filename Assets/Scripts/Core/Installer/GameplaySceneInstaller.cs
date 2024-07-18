using UnityEngine;
using Zenject;
using StarterAssets;
using MyProject;
using UnityEngine.EventSystems;


public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private Spawner spawner;

    public override void InstallBindings()
    {
        Container.Bind<StarterAssetsInputs>().FromInstance(starterAssetsInputs).AsSingle();
        
        Helper.spawner = spawner;
    }
}

using MyProject;
using States;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Mk3Installer : MonoInstaller
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    
    public override void InstallBindings()
    {
        // Container.Bind<IMuveHandler>().To<MoveSpiderHanler>().AsTransient().WhenInjectedInto<SpiderMediator>();

        Container.Bind<IMuveHandler>().To<MoveSpiderHanler>().AsTransient();

        // Container.Bind<IWeaponStateHandler>().To<SpiderWeaponSystem>().AsTransient();

        Container.Bind<NavMeshAgent>().FromInstance(navMeshAgent);
    }
    
}

namespace MyProject
{
    public class MoveSpiderSystem : MoveSpiderHanler, IMuveHandler
    {

    }
    public class SpiderWeaponSystem : SpiderWeaponHandler, IWeaponStateSystem
    {
        public IWeaponStates WeaponStates => throw new System.NotImplementedException();

    }
}

using ModestTree;
using UnityEngine;
using Zenject;

namespace Tool
{
    public class UIInventoryInstaller : MonoInstaller
    {
        // public Player playerMediator;
        public override void InstallBindings()
        {
            // Container.Bind<IMediator>().To<PlayerMediator>().AsCached().WhenInjectedInto<InventoryUI>();
        }
    }
}
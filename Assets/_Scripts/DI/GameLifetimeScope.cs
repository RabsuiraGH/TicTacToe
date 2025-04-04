using CodeBase.Config.PathConfig;
using EBus;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class GameLifetimeScope : MonoInstaller

    {
        public override void InstallBindings()
        {
            RegisterFactories(Container);
            RegisterServices(Container);
            RegisterEventBus(Container);
        }


        private static void RegisterEventBus(DiContainer container)
        {
            container.BindInterfacesTo<EBus.EventBus>().AsSingle();
        }

        private static void RegisterFactories(DiContainer container)
        {
            container
               .Bind<MarkCellFactory>()
               .AsSingle();
        }


        private static void RegisterServices(DiContainer container)
        {

            container.Bind<Game>().AsTransient();


            container
               .Bind<IGridService>()
               .To<GridService>()
               .AsSingle();

            container
               .Bind<BoardService>()
               .AsSingle();

            container
               .Bind<GameFieldService>()
               .AsSingle();
        }
    }
}
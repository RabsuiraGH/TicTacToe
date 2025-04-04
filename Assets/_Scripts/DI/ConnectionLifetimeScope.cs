using CodeBase.Services.AvatarPicker;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class ConnectionLifetimeScope : MonoInstaller
    {
        public override void InstallBindings()
        {
            RegisterServices(Container);
            RegisterFactories(Container);
            RegisterBuffers(Container);
        }


        private static void RegisterBuffers(DiContainer container)
        {
            container
               .Bind<PlayerDataBuffer>()
               .AsSingle();
        }


        private static void RegisterFactories(DiContainer container)
        {
            container
               .Bind<PlayerInfoUIFactory>()
               .AsSingle();
        }

        private static void RegisterServices(DiContainer container)
        {
            container
               .BindInterfacesTo<RoomService>()
               .AsSingle();

            container
               .BindInterfacesTo<PlayerConfigurationService>()
               .AsSingle();

            container
               .BindInterfacesTo<AvatarPickerService>()
               .AsSingle();
        }

    }
}
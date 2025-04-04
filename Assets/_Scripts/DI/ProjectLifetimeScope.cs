using CodeBase.Config.MarkConfig;
using Zenject;
using CodeBase.Config.PathConfig;
using Unity.Netcode;
using UnityEngine;

namespace CodeBase
{
    public class ProjectLifetimeScope : MonoInstaller
    {
        [SerializeField] private PathConfig _pathConfig;
        [SerializeField] private MarkAssetConfig _markAssetConfig;
        [SerializeField] private ConnectionService _connectionService;


        public override void InstallBindings()
        {
            RegisterConfigs(Container);
            RegisterServices(Container);
        }


        private void RegisterConfigs(DiContainer container)
        {
            container
               .BindInterfacesTo<PathConfig>()
               .FromInstance(_pathConfig)
               .AsSingle();

            container
               .Bind<MarkAssetConfig>()
               .FromInstance(_markAssetConfig)
               .AsSingle();
        }


        private static void RegisterServices(DiContainer container)
        {
            container
               .Bind<ConnectionService>()
               .AsSingle();
        }
    }
}
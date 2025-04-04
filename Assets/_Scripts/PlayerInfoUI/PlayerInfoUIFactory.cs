using CodeBase.Utilities.LoadAsset;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class PlayerInfoUIFactory : IFactory<PlayerInfoBoxUI>
    {
        private readonly DiContainer _container;
        private readonly string _pathToObject;


        [Inject]
        public PlayerInfoUIFactory(DiContainer container, IPlayerInfoUIPathConfig config)
        {
            _container = container;
            _pathToObject = config.PlayerInfoUIPath;
        }


        public PlayerInfoBoxUI Create()
        {
            GameObject instance = _container.InstantiatePrefab(LoadAssetUtility.Load<PlayerInfoBoxUI>(_pathToObject));

            PlayerInfoBoxUI markCell = instance.GetComponent<PlayerInfoBoxUI>();

            return markCell;
        }
    }
}
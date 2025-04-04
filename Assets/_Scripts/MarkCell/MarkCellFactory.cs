using CodeBase.Config.PathConfig;
using CodeBase.Utilities.LoadAsset;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class MarkCellFactory : IFactory<Vector2, Marker, Transform, MarkCell>
    {
        private readonly DiContainer _container;
        private readonly string _pathToObject;


        [Inject]
        public MarkCellFactory(DiContainer container, IMarkCellPathConfig config)
        {
            _container = container;
            _pathToObject = config.MarkCellPath;
        }


        public MarkCell Create(Vector2 position, Marker marker, Transform parent)
        {
            GameObject instance = _container.InstantiatePrefab(LoadAssetUtility.Load<MarkCell>(_pathToObject),
                                                               position, Quaternion.identity, parent);

            MarkCell markCell = instance.GetComponent<MarkCell>();
            if (markCell != null)
            {
                markCell.Initialize(marker);
            }

            return markCell;
        }
    }
}
using CodeBase.Config.PathConfig;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class MarkCellFactory : IFactory<Vector2, Marker, MarkCell>
    {
        private readonly DiContainer _container;
        private readonly string _pathToObject;
        private readonly Transform _marksRoot;
        private const string ROOT_NAME = "MarksRoot";


        [Inject]
        public MarkCellFactory(DiContainer container, IMarkCellPathConfig config)
        {
            _container = container;
            _pathToObject = config.MarkCellPath;
            _marksRoot = GameObject.Find(ROOT_NAME)?.transform ?? new GameObject(ROOT_NAME).transform;
        }


        public MarkCell Create(Vector2 position, Marker marker)
        {
            GameObject instance = _container.InstantiatePrefab(LoadAssetUtility.Load<MarkCell>(_pathToObject),
                                                               position, Quaternion.identity, _marksRoot);

            MarkCell markCell = instance.GetComponent<MarkCell>();
            if (markCell != null)
            {
                markCell.Initialize(marker);
            }

            return markCell;
        }
    }
}
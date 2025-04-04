using InspectorPathField;
using UnityEngine;

namespace CodeBase.Config.PathConfig
{
    [CreateAssetMenu(fileName = "PathConfig", menuName = "Configs/PathConfig")]
    public class PathConfig : ScriptableObject, IMarkCellPathConfig, IPlayerInfoUIPathConfig
    {
        [field: SerializeField] public PathField MarkCellPath { get; private set; }

        [field: SerializeField] public PathField PlayerInfoUIPath { get; private set; }
    }
}
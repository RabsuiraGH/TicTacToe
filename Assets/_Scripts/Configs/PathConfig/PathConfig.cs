using UnityEngine;

namespace CodeBase.Config.PathConfig
{
    [CreateAssetMenu(fileName = "PathConfig", menuName = "Configs/PathConfig")]
    public class PathConfig : ScriptableObject, IMarkCellPathConfig
    {
        [field: SerializeField] public string MarkCellPath { get; private set; }
    }


}
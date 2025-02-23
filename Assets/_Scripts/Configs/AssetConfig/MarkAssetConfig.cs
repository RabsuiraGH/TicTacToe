using System;
using InspectorPathField;
using UnityEngine;

namespace CodeBase.Config.MarkConfig
{
    [CreateAssetMenu(fileName = "MarkAssetConfig", menuName = "Configs/AssetConfigs/Mark Asset Config", order = 0)]
    [System.Serializable]
    public class MarkAssetConfig : ScriptableObject
    {
        [SerializeField] private PathField _crossSpritePath;
        [SerializeField] private PathField _circleSpritePath;
        [SerializeField] private PathField _triangleSpritePath;
        [SerializeField] private PathField _squareSpritePath;


        public Sprite GetSprite(Marker mark)
        {
            return mark switch
            {
                Marker.Cross => LoadAssetUtility.Load<Sprite>(_crossSpritePath),
                Marker.Circle => LoadAssetUtility.Load<Sprite>(_circleSpritePath),
                Marker.Triangle => LoadAssetUtility.Load<Sprite>(_triangleSpritePath),
                Marker.Square => LoadAssetUtility.Load<Sprite>(_squareSpritePath),
                var _ => throw new ArgumentOutOfRangeException(nameof(mark), mark, "No sprite for mark available")
            };
        }
    }
}
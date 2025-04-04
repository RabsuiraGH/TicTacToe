using CodeBase.Config.MarkConfig;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase
{
    public class PlayerInfoBoxUI : MonoBehaviour
    {
        [SerializeField] private Image _avatar;
        [SerializeField] private Image _marker;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _nameText;

        [SerializeField] private PlayerNetworkData _playerData;
        private MarkAssetConfig _markAssetConfig;


        [Inject]
        public void Construct(MarkAssetConfig markAssetConfig)
        {
            _markAssetConfig = markAssetConfig;
        }


        public void UpdateAvatar(byte[] textureBytes)
        {
            Texture2D avatarTexture = new(1, 1);
            avatarTexture.LoadImage(textureBytes);
            _avatar.sprite = Sprite.Create(avatarTexture,
                                           new Rect(0, 0, avatarTexture.width, avatarTexture.height),
                                           new Vector2(0.5f, 0.5f));
        }


        public void UpdateId(ulong _, ulong newId) =>
            _idText.text = $"Id: {newId}";


        public void UpdateMarker(int _, int newMarker) =>
            _marker.sprite = _markAssetConfig.GetSprite((Marker)newMarker);


        public void UpdateName(FixedString32Bytes _, FixedString32Bytes newName) => _nameText.text = newName.ToString();
    }
}
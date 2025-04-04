using CodeBase.Utilities.ImagePicker;
using UnityEngine;

namespace CodeBase.Services.AvatarPicker
{
    public sealed class AvatarPickerService : IAvatarPickerService
    {
        private Texture2D _avatarTexture;
        private const string DEFAULT_FILE_NAME = "Avatar.png";

        public Texture2D GetAvatar() => _avatarTexture;

        public byte[] GetAvatarBytes() => _avatarTexture?.EncodeToPNG();


        public bool TryPickAvatar()
        {
            return ImagePickerUtility.TryPickImage((texture) =>
            {
                _avatarTexture = texture;
                ImagePickerUtility.SaveImage(texture, DEFAULT_FILE_NAME);
            }, 512);
        }


        public bool TryLoadSavedAvatar() => ImagePickerUtility.TryLoadImage(DEFAULT_FILE_NAME, out _avatarTexture);
    }
}
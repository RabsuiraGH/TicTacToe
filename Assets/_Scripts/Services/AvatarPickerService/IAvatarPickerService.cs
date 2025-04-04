using UnityEngine;

namespace CodeBase.Services.AvatarPicker
{
    /// <summary>
    /// Provides an API for picking an avatar from the device's storage.
    /// </summary>
    public interface IAvatarPickerService
    {
        /// <summary>
        /// Returns picked texture.
        /// </summary>
        public Texture2D GetAvatar();


        /// <summary>
        /// Returns selected texture as bytes. Null if texture is not picked.
        /// </summary>
        public byte[] GetAvatarBytes();


        /// <summary>
        /// Open dialog for picking image from device storage.
        /// <para>Places picked image to <b>buffer</b>.</para>
        /// </summary>
        /// <returns>Returns pick status.</returns>
        public bool TryPickAvatar();


        /// <summary>
        /// Loads an image from device storage.
        /// </summary>
        /// <returns>True if load successfully. False otherwise</returns>
        public bool TryLoadSavedAvatar();
    }
}
using UnityEngine;

namespace CodeBase.EventBusSignals
{
    public sealed class S_OnAvatarPicked
    {
        public readonly Texture2D AvatarTexture;
        public S_OnAvatarPicked(Texture2D avatarTexture)
        {
            AvatarTexture = avatarTexture;
        }
    }
}
using System;
using EBus;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class PlayerDataBuffer
    {
        public PlayerData PlayerData;
        public ChangedData ChangedData;

        public int PlayerMark = 1;

        public event Action<PlayerDataBuffer> OnPlayerDataBufferUpdated;

        public void NotifyAboutUpdate() => OnPlayerDataBufferUpdated?.Invoke(this);
    }

    [Serializable]
    public struct PlayerData : INetworkSerializable
    {
        public string PlayerName;
        public byte[] PlayerAvatar;


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
        }
    }

    [Flags]
    public enum ChangedData
    {
        None = 0,
        Name = 1 << 0,
        Avatar = 1 << 1,
    }
}
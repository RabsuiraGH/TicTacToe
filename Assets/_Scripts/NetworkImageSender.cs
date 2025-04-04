using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CodeBase
{
    public class NetworkImageSender : NetworkBehaviour
    {
        public event Action<byte[]> OnAvatarReceived;
        private const int CHUNK_SIZE = 1024;
        private readonly Dictionary<ulong, List<byte>> _playerAvatarData = new();


        public void SendAvatar(byte[] playerAvatar, bool toServer)
        {
            if (playerAvatar == null || playerAvatar.Length == 0)
            {
                Debug.LogError("No data to send.");
                return;
            }

            int totalChunks = Mathf.CeilToInt((float)playerAvatar.Length / CHUNK_SIZE);

            for (int i = 0; i < totalChunks; i++)
            {
                int start = i * CHUNK_SIZE;
                int length = Mathf.Min(CHUNK_SIZE, playerAvatar.Length - start);
                byte[] chunk = new byte[length];
                Array.Copy(playerAvatar, start, chunk, 0, length);
                bool isLastChunk = (i == totalChunks - 1);

                if (toServer)
                {
                    SendAvatarChunkToServer_Rpc(OwnerClientId, chunk, isLastChunk);
                }
                else
                {
                    SendAvatarChunkToClient_Rpc(OwnerClientId, chunk, isLastChunk);
                }
            }
        }


        [Rpc(SendTo.NotServer)]
        private void SendAvatarChunkToClient_Rpc(ulong clientId, byte[] chunk, bool isLastChunk)
        {
            ReceiveAvatarChunk(clientId, chunk, isLastChunk);
        }


        [Rpc(SendTo.Server)]
        private void SendAvatarChunkToServer_Rpc(ulong clientId, byte[] chunk, bool isLastChunk)
        {
            ReceiveAvatarChunk(clientId, chunk, isLastChunk);
        }


        private void ReceiveAvatarChunk(ulong fromClientId, byte[] chunk, bool isLastChunk)
        {
            if (!_playerAvatarData.ContainsKey(fromClientId))
            {
                _playerAvatarData[fromClientId] = new List<byte>();
            }

            _playerAvatarData[fromClientId].AddRange(chunk);

            if (isLastChunk)
            {
                byte[] avatar = _playerAvatarData[fromClientId].ToArray();
                _playerAvatarData.Remove(fromClientId);

                OnAvatarReceived?.Invoke(avatar);
                if (IsServer) SendAvatar(avatar, false);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        public static List<PlayerNetworkData> AllInstances { get; } = new();
        public static PlayerNetworkData LocalInstance { get; private set; }

        public NetworkVariable<ulong> PlayerId = new();
        public NetworkVariable<FixedString32Bytes> PlayerName = new();
        public NetworkVariable<int> PlayerMarker = new();
        public byte[] PlayerAvatar;

        public event Action<byte[]> OnAvatarUpdated;

        private PlayerDataBuffer _playerDataBuffer;

        private NetworkImageSender _networkImageSender;


        public static PlayerNetworkData GetDataById(ulong playerId) =>
            AllInstances.FirstOrDefault(data => data.OwnerClientId == playerId);


        public override void OnNetworkSpawn()
        {
            if (!AllInstances.Contains(this))
                AllInstances.Add(this);

            if (IsOwner)
            {
                LocalInstance = this;
            }

            if (_networkImageSender == null && gameObject.TryGetComponent(out NetworkImageSender networkImageSender))
            {
                _networkImageSender = networkImageSender;
                _networkImageSender.OnAvatarReceived += GetAvatar;
            }
        }


        public void Initialize(PlayerDataBuffer playerDataBuffer)
        {
            _playerDataBuffer = playerDataBuffer;

            SetDataOnServer(playerDataBuffer);

            playerDataBuffer.OnPlayerDataBufferUpdated += SetDataOnServer;
        }


        public void OnThisPlayerFullyInitialized()
        {
            RequestAllPlayersToSendTheirDataToClients_Rpc(this.OwnerClientId);
        }


        [Rpc(SendTo.NotMe)]
        private void TriggerDataUpdate_Rpc()
        {
            TriggerDataUpdate();
        }


        private void SetDataOnServer(PlayerDataBuffer playerDataBuffer)
        {
            if ((playerDataBuffer.ChangedData & ChangedData.Avatar) != 0)
            {
                _networkImageSender.SendAvatar(playerDataBuffer.PlayerData.PlayerAvatar, true);
            }

            SetDataOnServer_Rpc(OwnerClientId, playerDataBuffer.PlayerData, playerDataBuffer.PlayerMark);
        }


        private void TriggerDataUpdate()
        {
            PlayerId.OnValueChanged.Invoke(PlayerId.Value, PlayerId.Value);
            PlayerName.OnValueChanged.Invoke(PlayerName.Value, PlayerName.Value);
            PlayerMarker.OnValueChanged.Invoke(PlayerMarker.Value, PlayerMarker.Value);
        }


        [Rpc(SendTo.Server)]
        private void SetDataOnServer_Rpc(ulong id, PlayerData playerData, int playerMark)
        {
            PlayerId.Value = id;
            PlayerName.Value = playerData.PlayerName;
            PlayerMarker.Value = (int)playerMark;
            TriggerDataUpdate();
        }


        [Rpc(SendTo.Server)]
        private void RequestAllPlayersToSendTheirDataToClients_Rpc(ulong excludeId)
        {
            foreach (PlayerNetworkData data in AllInstances)
            {
                if (data.OwnerClientId == excludeId) continue;

                data._networkImageSender.SendAvatar(data.PlayerAvatar, false);
                data.TriggerDataUpdate_Rpc();
            }
        }


        private void GetAvatar(byte[] avatar)
        {
            PlayerAvatar = avatar;
            OnAvatarUpdated?.Invoke(avatar);
        }


        public override void OnDestroy()
        {
            AllInstances.Remove(this);
            if (_playerDataBuffer != null)
            {
                _playerDataBuffer.OnPlayerDataBufferUpdated -= SetDataOnServer;
            }

            if (_networkImageSender != null)
            {
                _networkImageSender.OnAvatarReceived -= GetAvatar;
            }

            base.OnDestroy();
        }
    }
}
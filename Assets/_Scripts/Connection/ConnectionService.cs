using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace CodeBase
{
    public class ConnectionService
    {
        public ulong ClientId => NetworkManager.Singleton.LocalClientId;
        public bool IsConnectedClient => NetworkManager.Singleton.IsConnectedClient;

        public bool IsHost => NetworkManager.Singleton.IsHost;
        public bool IsServer => NetworkManager.Singleton.IsServer;
        public bool IsClient => NetworkManager.Singleton.IsClient;

        public ConnectionNotifier ConnectionNotifier => ConnectionNotifier.Instance;

        public event Action<ulong> OnSpecificClientConnected;
        public event Action<ulong> OnSpecificClientDisconnected;
        public event Action OnConnectionFailed;
        public event Action<string> OnInvalidIpAddress;


        public ConnectionService()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.Singleton.OnTransportFailure += OnConnectionFailedCallback;
        }


        ~ConnectionService()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.Singleton.OnTransportFailure -= OnConnectionFailedCallback;
        }


        private void OnConnectionFailedCallback()
        {
            OnConnectionFailed?.Invoke();
        }


        private void OnClientDisconnectCallback(ulong playerId)
        {
            OnSpecificClientDisconnected?.Invoke(playerId);
        }


        private void OnClientConnectedCallback(ulong playerId)
        {
            OnSpecificClientConnected?.Invoke(playerId);
        }


        public bool TryStartHost(string roomIP)
        {
#if UNITY_EDITOR
            if (roomIP == string.Empty) roomIP = "0.0.0.0";
            else if (!IsValidAddress(roomIP)) return false;
#else
            if (!IsValidAddress(roomIP)) return false;
#endif

            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = roomIP;
            NetworkManager.Singleton.StartHost();
            return true;
        }


        public bool TryStartClient(string roomIP)
        {
#if UNITY_EDITOR
            if (roomIP == string.Empty) roomIP = "127.0.0.1";
            else if (!IsValidAddress(roomIP)) return false;
#else
            if (!IsValidAddress(roomIP)) return false;
#endif
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = roomIP;
            NetworkManager.Singleton.StartClient();

            return true;
        }


        public void DisconnectPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
        }


        public void Disconnect()
        {
            ConnectionNotifier.Disconnect();
            NetworkManager.Singleton.Shutdown();
        }


        private bool IsValidAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;

            string[] parts = ip.Split('.');
            if (parts.Length != 4) return false;

            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0 || num > 255)
                {
                    OnInvalidIpAddress?.Invoke(ip);
                    return false;
                }
            }

            return true;
        }
    }
}
using System;
using Unity.Netcode;
using UnityEngine;

namespace CodeBase
{
    public class ConnectionNotifier : NetworkBehaviour
    {
        public static ConnectionNotifier Instance { get; private set; }

        public event Action<ulong> OnAnyPlayerConnected;
        public event Action<ulong> OnAnyPlayerDisconnected;
        public event Action OnServerDisconnected;


        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }


        public override void OnNetworkSpawn()
        {
            if (NetworkManager.IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }


        public override void OnNetworkDespawn()
        {
            if (NetworkManager.IsServer && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }


        private void OnClientDisconnected(ulong clientId)
        {
            NotifyAboutDisconnection_Rpc(clientId);
        }


        private void OnClientConnected(ulong clientId)
        {
            NotifyAboutConnection_Rpc(clientId);
        }


        [Rpc(SendTo.Everyone)]
        private void NotifyAboutConnection_Rpc(ulong connectedPlayerId)
        {
            OnAnyPlayerConnected?.Invoke(connectedPlayerId);
            Debug.Log(($"Network Notification: Player {connectedPlayerId} connected."));
        }


        [Rpc(SendTo.NotMe)]
        private void NotifyAboutDisconnection_Rpc(ulong disconnectedPlayerId)
        {
            OnAnyPlayerDisconnected?.Invoke(disconnectedPlayerId);
            Debug.Log(($"Network Notification: Player {disconnectedPlayerId} disconnected."));
        }


        [Rpc(SendTo.NotServer)]
        private void NotifyAboutServerDisconnection_Rpc()
        {
            if (NetworkManager.IsServer) return;
            OnServerDisconnected?.Invoke();
            Debug.Log(($"Network Notification: Server disconnected."));
        }


        public void Disconnect()
        {
            if (NetworkManager.IsServer) NotifyAboutServerDisconnection_Rpc();
        }
    }
}
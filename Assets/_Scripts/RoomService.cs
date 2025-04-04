using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.EventBus.Signals;
using CodeBase.EventBusSignals;
using EBus;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class RoomService : IInitializable
    {
        private IEventBus _eventBus;
        private ConnectionService _connectionService;
        private PlayerDataBuffer _playerDataBuffer;

        private CancellationTokenSource _cancellationTokenSource;
        private PlayerInfoUIFactory _playerInfoUIFactory;

        private const int MAX_SEARCH_TIME_MS = 5000;


        public void Initialize()
        {
        }


        [Inject]
        public void Construct(IEventBus eventBus, ConnectionService connectionService,
                              PlayerDataBuffer playerDataBuffer, PlayerInfoUIFactory playerInfoUIFactory)
        {
            _playerInfoUIFactory = playerInfoUIFactory;

            _playerDataBuffer = playerDataBuffer;

            _connectionService = connectionService;
            _connectionService.OnSpecificClientConnected += RoomWasFound;
            _connectionService.ConnectionNotifier.OnAnyPlayerConnected += AddPlayer;
            _connectionService.ConnectionNotifier.OnAnyPlayerDisconnected += RemovePlayer;
            _connectionService.ConnectionNotifier.OnServerDisconnected += LeaveRoom;


            _eventBus = eventBus;

            _eventBus.Subscribe<S_OnRoomCreationRequested>(TryCreateRoom);
            _eventBus.Subscribe<S_OnRoomJoinRequested>(TryJoinRoom);
            _eventBus.Subscribe<S_OnJoinRoomSearchStarted>(WaitSearchForRoom);

            _eventBus.Subscribe<S_OnPlayerInfoBoxDeleteRequested>(DeletePlayerInfoBoxUI);

            _eventBus.Subscribe<S_OnRoomLeaveRequested>(LeaveRoom);

            _eventBus.Subscribe<S_OnSearchLeaveRequested>(LeaveRoom);
        }


        private void TryCreateRoom(S_OnRoomCreationRequested onRoomCreationRequested)
        {
            string id = onRoomCreationRequested.RoomId;
            bool success = _connectionService.TryStartHost(id);

            _eventBus.Invoke(new S_OnRoomCreationStarted(success, id));
        }


        private void TryJoinRoom(S_OnRoomJoinRequested onRoomJoinRequested)
        {
            string id = onRoomJoinRequested.RoomId;
            bool success = _connectionService.TryStartClient(id);

            _eventBus.Invoke(new S_OnJoinRoomSearchStarted(success, id));
        }


        private void AddPlayer(ulong playerId)
        {
            PlayerNetworkData playerData = PlayerNetworkData.GetDataById(playerId);

            if (playerData == null) return;

            PlayerInfoBoxUI playerInfoBoxUI = CreatePlayerInfoBoxUI(playerData);

            if (_connectionService.ClientId == playerId)
            {
                InitializeSelf(playerId, playerData);
            }

            _eventBus.Invoke(new S_OnPlayerInfoBoxCreated(playerId, playerInfoBoxUI));
        }


        private void InitializeSelf(ulong playerId, PlayerNetworkData playerData)
        {
            playerData.Initialize(_playerDataBuffer);

            foreach (PlayerNetworkData data in PlayerNetworkData.AllInstances)
            {
                if (data.OwnerClientId == playerId) continue;

                PlayerInfoBoxUI otherPlayerInfoBox = CreatePlayerInfoBoxUI(data);
                _eventBus.Invoke(new S_OnPlayerInfoBoxCreated(data.OwnerClientId, otherPlayerInfoBox));
            }

            playerData.OnThisPlayerFullyInitialized();
        }


        private void RemovePlayer(ulong playerId)
        {
            _eventBus.Invoke(new S_OnPlayerDisconnected(playerId, _connectionService.ClientId == playerId));
        }


        private PlayerInfoBoxUI CreatePlayerInfoBoxUI(PlayerNetworkData playerData)
        {
            PlayerInfoBoxUI playerInfoBoxUI = _playerInfoUIFactory.Create();
            playerData.OnAvatarUpdated += playerInfoBoxUI.UpdateAvatar;
            playerData.PlayerName.OnValueChanged += playerInfoBoxUI.UpdateName;
            playerData.PlayerId.OnValueChanged += playerInfoBoxUI.UpdateId;
            playerData.PlayerMarker.OnValueChanged += playerInfoBoxUI.UpdateMarker;
            return playerInfoBoxUI;
        }


        private static void DeletePlayerInfoBoxUI(S_OnPlayerInfoBoxDeleteRequested signal)
        {
            PlayerNetworkData playerData = PlayerNetworkData.GetDataById(signal.PlayerId);
            playerData.OnAvatarUpdated -= signal.PlayerInfoBoxUI.UpdateAvatar;
            playerData.PlayerName.OnValueChanged -= signal.PlayerInfoBoxUI.UpdateName;
            playerData.PlayerId.OnValueChanged -= signal.PlayerInfoBoxUI.UpdateId;
            playerData.PlayerMarker.OnValueChanged -= signal.PlayerInfoBoxUI.UpdateMarker;
            Object.Destroy(signal.PlayerInfoBoxUI.gameObject);
        }


        private void RoomWasFound(ulong playerId)
        {
            if (playerId != _connectionService.ClientId) return;
            _cancellationTokenSource?.Cancel();
            _eventBus.Invoke(new S_OnRoomFound());
        }


        private async void WaitSearchForRoom(S_OnJoinRoomSearchStarted signal)
        {
            if (!signal.Succeed) return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            // Wait for some time while room in search
            try
            {
                await Task.Delay(MAX_SEARCH_TIME_MS, token);
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, handle cancellation if necessary
                return;
            }

            if (_connectionService.IsConnectedClient) return;

            // If time has passed, and we are not connected, forcibly stop client
            LeaveRoom();
        }


        private void LeaveRoom()
        {
            _cancellationTokenSource?.Cancel();
            _connectionService.Disconnect();
            _eventBus.Invoke(new S_OnRoomLeaved());
        }


        private void LeaveRoom<T>(T leaveSignal)
        {
            LeaveRoom();
        }


        ~RoomService()
        {
            _cancellationTokenSource?.Cancel();

            _connectionService.OnSpecificClientConnected -= RoomWasFound;
            _connectionService.ConnectionNotifier.OnAnyPlayerConnected -= AddPlayer;
            _connectionService.ConnectionNotifier.OnAnyPlayerDisconnected -= RemovePlayer;


            _eventBus.Unsubscribe<S_OnRoomCreationRequested>(TryCreateRoom);
            _eventBus.Unsubscribe<S_OnRoomJoinRequested>(TryJoinRoom);
            _eventBus.Unsubscribe<S_OnJoinRoomSearchStarted>(WaitSearchForRoom);


            _eventBus.Unsubscribe<S_OnPlayerInfoBoxDeleteRequested>(DeletePlayerInfoBoxUI);


            _eventBus.Unsubscribe<S_OnRoomLeaveRequested>(LeaveRoom);

            _eventBus.Unsubscribe<S_OnSearchLeaveRequested>(LeaveRoom);
        }
    }
}
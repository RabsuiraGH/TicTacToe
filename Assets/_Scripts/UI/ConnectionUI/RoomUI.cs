using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeBase.Config.PathConfig;
using CodeBase.EventBus.Signals;
using CodeBase.EventBusSignals;
using EBus;
using InspectorPathField;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase
{
    public class RoomUI : UITemplate
    {
        [SerializeField] private Transform _playerInfoTransform;
        [SerializeField] private Button _leaveRoomButton;
        [SerializeField] private Button _startGameButton;

        private readonly Dictionary<ulong, PlayerInfoBoxUI> _playersInfo = new();

        private IEventBus _eventBus;


        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<S_OnRoomCreationStarted>(ShowOnCreate);
            _eventBus.Subscribe<S_OnRoomFound>(ShowOnFound);
            _eventBus.Subscribe<S_OnRoomLeaved>(CloseRoomUI);
            _eventBus.Subscribe<S_OnPlayerDisconnected>(OnAnyPlayerDisconnected);

            _eventBus.Subscribe<S_OnPlayerInfoBoxCreated>(AddPlayerUIBox);


            _startGameButton.onClick.AddListener(RequestToStartGame);
            _leaveRoomButton.onClick.AddListener(RequestToLeaveRoom);
        }


        private void AddPlayerUIBox(S_OnPlayerInfoBoxCreated signal)
        {
            signal.PlayerInfoBoxUI.transform.SetParent(_playerInfoTransform);
            signal.PlayerInfoBoxUI.transform.localScale = Vector3.one;

            // If no duplicates, just add
            if (!_playersInfo.TryGetValue(signal.PlayerId, out PlayerInfoBoxUI oldPlayerInfoBoxUI))
            {
                _playersInfo.Add(signal.PlayerId, signal.PlayerInfoBoxUI);
            }
            else // Else override existing
            {
                Destroy(oldPlayerInfoBoxUI.gameObject);
                _playersInfo[signal.PlayerId] = signal.PlayerInfoBoxUI;
            }
        }


        private void RemovePlayerUIBox(ulong playerId)
        {
            if (!_playersInfo.TryGetValue(playerId, out PlayerInfoBoxUI ui)) return;

            _eventBus.Invoke(new S_OnPlayerInfoBoxDeleteRequested(playerId, ui));
            _playersInfo.Remove(playerId);
        }


        private void RequestToStartGame()
        {
        }


        private void OnAnyPlayerDisconnected(S_OnPlayerDisconnected signal)
        {
            RemovePlayerUIBox(signal.DisconnectedPlayerId);
        }


        private void RequestToLeaveRoom() => _eventBus.Invoke(new S_OnRoomLeaveRequested());


        private void CloseRoomUI(S_OnRoomLeaved signal)
        {
            foreach (ulong id in _playersInfo.Keys)
            {
                _eventBus.Invoke(new S_OnPlayerInfoBoxDeleteRequested(id, _playersInfo[id]));
            }

            _playersInfo.Clear();
            Hide();
        }


        private void ShowOnFound(object _) => Show();


        private void ShowOnCreate(S_OnRoomCreationStarted signal)
        {
            if (!signal.Succeed) return;
            Show();
        }


        private void OnDestroy()
        {
            _eventBus.Unsubscribe<S_OnRoomCreationStarted>(ShowOnCreate);
            _eventBus.Unsubscribe<S_OnRoomFound>(ShowOnFound);
            _eventBus.Unsubscribe<S_OnRoomLeaved>(CloseRoomUI);
            _eventBus.Unsubscribe<S_OnPlayerDisconnected>(OnAnyPlayerDisconnected);


            _startGameButton.onClick.RemoveListener(RequestToStartGame);
            _leaveRoomButton.onClick.RemoveListener(RequestToLeaveRoom);
        }
    }
}
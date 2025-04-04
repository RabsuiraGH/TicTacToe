using System;
using CodeBase.EventBus.Signals;
using CodeBase.EventBusSignals;
using EBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase
{
    public class JoinRoomUI : UITemplate
    {
        [SerializeField] private Button _joinRoomButton;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private TMP_InputField _roomIdField;

        private IEventBus _eventBus;


        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<S_OnRoomLeaved>(Show); // Whenever we leave the room, we would like to show join room UI
            _eventBus.Subscribe<S_OnRoomCreationStarted>(ReactToRoomCreation);
            _eventBus.Subscribe<S_OnJoinRoomSearchStarted>(ReactToJoinRoomSearchStarted);

            _joinRoomButton.onClick.AddListener(RequestToJoinRoom);
            _createRoomButton.onClick.AddListener(RequestToCreateRoom);
        }


        /// <summary>
        /// Requests the creation of a new room with the specified room ID.
        /// </summary>
        private void RequestToCreateRoom() => _eventBus.Invoke(new S_OnRoomCreationRequested(_roomIdField.text));


        /// <summary>
        /// Requests the joining of an existing room with the specified room ID.
        /// </summary>
        private void RequestToJoinRoom() => _eventBus.Invoke(new S_OnRoomJoinRequested(_roomIdField.text));


        private void ReactToRoomCreation(S_OnRoomCreationStarted onRoomCreationStarted)
        {
            if (onRoomCreationStarted.Succeed)
            {
                Hide();
                return;
            }

            Notify($"invalid ip {onRoomCreationStarted.RoomIP}");
        }


        private void ReactToJoinRoomSearchStarted(S_OnJoinRoomSearchStarted onJoinRoomSearchStarted)
        {
            if (onJoinRoomSearchStarted.Succeed)
            {
                Hide();
                return;
            }

            Notify($"invalid ip {onJoinRoomSearchStarted.RoomIP}");
        }


        private void Notify(string message)
        {
            Vector2 position = new(0, -Screen.height * 0.1f);
            Vector2 size = new(Screen.width * 0.8f, Screen.height * 0.1f);
            Vector2 anchor = new(0.5f, 1f);

            UIPopupUtility.CreateTextPopup(message, new Rect(position, size), 3f, _parentTransform, anchor);
        }


        private void Show(S_OnRoomLeaved obj) => Show();


        private void OnDestroy()
        {
            _eventBus.Unsubscribe<S_OnRoomLeaved>(Show);
            _eventBus.Unsubscribe<S_OnRoomCreationStarted>(ReactToRoomCreation);
            _eventBus.Unsubscribe<S_OnJoinRoomSearchStarted>(ReactToJoinRoomSearchStarted);

            _joinRoomButton.onClick.RemoveListener(RequestToJoinRoom);
            _createRoomButton.onClick.RemoveListener(RequestToCreateRoom);
        }
    }
}
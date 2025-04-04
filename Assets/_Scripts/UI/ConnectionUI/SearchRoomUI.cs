using CodeBase.EventBus.Signals;
using CodeBase.EventBusSignals;
using EBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase
{
    public class SearchRoomUI : UITemplate
    {
        [SerializeField] private TextMeshProUGUI _searchingLabel;
        [SerializeField] private Image _searchingImage;
        [SerializeField] private Button _leaveSearchButton;

        private IEventBus _eventBus;
        private ConnectionService _connectionService;


        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<S_OnJoinRoomSearchStarted>(ReactToJoinRoomSearchStarted);
            _eventBus.Subscribe<S_OnRoomLeaved>(OnSearchLeaved);
            _eventBus.Subscribe<S_OnRoomFound>(Hide);

            _leaveSearchButton.onClick.AddListener(RequestToLeaveSearch);
        }


        private void ReactToJoinRoomSearchStarted(S_OnJoinRoomSearchStarted signal)
        {
            if(!signal.Succeed) return;
            Show();
            // TODO: Play cool animation of search
            _searchingLabel.text = $"Searching for room...\nIP: {signal.RoomIP}";
        }


        private void OnSearchLeaved<T>(T _) => Hide();

        private void RequestToLeaveSearch() => _eventBus.Invoke(new S_OnSearchLeaveRequested());

        private void Hide<T>(T _) => Hide();


        private void OnDestroy()
        {
            _eventBus.Unsubscribe<S_OnJoinRoomSearchStarted>(ReactToJoinRoomSearchStarted);
            _eventBus.Unsubscribe<S_OnRoomLeaved>(OnSearchLeaved);
            _eventBus.Unsubscribe<S_OnRoomFound>(Hide);

            _leaveSearchButton.onClick.RemoveListener(RequestToLeaveSearch);
        }
    }
}
using System.Linq;
using CodeBase.EventBus.Signals;
using CodeBase.EventBusSignals;
using EBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase
{
    public sealed class PlayerConfigurationUI : UITemplate
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _selectAvatarButton;

        [SerializeField] private RawImage _avatarImage;

        [SerializeField] private TMP_InputField _playerNameField;

        private IEventBus _eventBus;


        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;

            _eventBus.Subscribe<S_OnAvatarPicked>(ApplyAvatar);
            _eventBus.Subscribe<S_OnPlayerDataUpdated>(FillUIWithData);

            _openButton.onClick.AddListener(Show);
            _selectAvatarButton.onClick.AddListener(RequestToPickAvatar);
            _closeButton.onClick.AddListener(RequestToApplyNewSettings);
        }


        private void ApplyAvatar(S_OnAvatarPicked signal) => _avatarImage.texture = signal.AvatarTexture;

        private void RequestToPickAvatar() => _eventBus.Invoke(new S_OnAvatarPickRequested());


        private void RequestToApplyNewSettings()
        {
            _eventBus.Invoke(new S_OnPlayerConfigurationSaveRequested(_playerNameField.text));
            Hide();
        }


        private void FillUIWithData(S_OnPlayerDataUpdated signal)
        {
            _playerNameField.text = signal.PlayerData.PlayerName;

            if (signal.PlayerData.PlayerAvatar != null && signal.PlayerData.PlayerAvatar.Any())
            {
                Texture2D texture = new(256, 256);
                texture.LoadImage(signal.PlayerData.PlayerAvatar);
                _avatarImage.texture = texture;
            }
        }


        public override void Show()
        {
            base.Show();
            _eventBus.Invoke(new S_OnPlayerDataRequested());
            _openButton.gameObject.SetActive(false);
        }


        public override void Hide()
        {
            base.Hide();
            _openButton.gameObject.SetActive(true);
        }


        private void OnDestroy()
        {
            _eventBus.Unsubscribe<S_OnAvatarPicked>(ApplyAvatar);
            _eventBus.Unsubscribe<S_OnPlayerDataUpdated>(FillUIWithData);


            _openButton.onClick.RemoveListener(Show);
            _selectAvatarButton.onClick.RemoveListener(RequestToPickAvatar);
            _closeButton.onClick.RemoveListener(RequestToApplyNewSettings);
        }
    }
}
using System.Linq;
using CodeBase.EventBus.Signals;
using CodeBase.EventBusSignals;
using CodeBase.Services.AvatarPicker;
using CodeBase.Utilities.JsonSaveLoad;
using EBus;
using Unity.VisualScripting;
using Zenject;
using IInitializable = Zenject.IInitializable;

namespace CodeBase
{
    public sealed class PlayerConfigurationService : IInitializable
    {
        private IEventBus _eventBus;
        private PlayerDataBuffer _playerDataBuffer;
        private IAvatarPickerService _avatarPickerService;
        private bool _firstLoad = true;

        private bool _avatarChanged = false;

        private const string PLAYER_DATA_SAVE_PATH = "PlayerData.json";


        [Inject]
        public void Construct(IEventBus eventBus, PlayerDataBuffer playerDataBuffer,
                              IAvatarPickerService avatarPickerService)
        {
            _eventBus = eventBus;
            _playerDataBuffer = playerDataBuffer;
            _avatarPickerService = avatarPickerService;

            Load();

            _eventBus.Subscribe<S_OnPlayerConfigurationSaveRequested>(UpdatePlayer);
            _eventBus.Subscribe<S_OnAvatarPickRequested>(PickAvatar);
            _eventBus.Subscribe<S_OnPlayerDataRequested>(OnDataRequested);
        }


        private void OnDataRequested(S_OnPlayerDataRequested signal)
        {
            if (_firstLoad)
            {
                Load();
            }

            // Notify about update
            _eventBus.Invoke(new S_OnPlayerDataUpdated(_playerDataBuffer.PlayerData));
        }


        private void PickAvatar(S_OnAvatarPickRequested signal)
        {
            if (!_avatarPickerService.TryPickAvatar()) return;

            _playerDataBuffer.PlayerData.PlayerAvatar = _avatarPickerService.GetAvatarBytes();
            _eventBus.Invoke(new S_OnAvatarPicked(_avatarPickerService.GetAvatar()));
            _avatarChanged = true;
        }


        private void UpdatePlayer(S_OnPlayerConfigurationSaveRequested signal)
        {
            ChangedData changed = ChangedData.None;

            if (_playerDataBuffer.PlayerData.PlayerName == null ||
                _playerDataBuffer.PlayerData.PlayerName != signal.PlayerName)
            {
                _playerDataBuffer.PlayerData.PlayerName = signal.PlayerName;
                changed |= ChangedData.Name;
            }

            if (_avatarChanged)
            {
                _playerDataBuffer.PlayerData.PlayerAvatar = _avatarPickerService.GetAvatarBytes();
                _avatarChanged = false;
                changed |= ChangedData.Avatar;
            }

            if (changed != 0)
            {
                _playerDataBuffer.ChangedData = changed;
                _playerDataBuffer.NotifyAboutUpdate();
                Save();
            }
        }


        private void Save() => JsonSaveLoadUtility.TrySave(_playerDataBuffer.PlayerData, PLAYER_DATA_SAVE_PATH, true);


        private void Load()
        {
            _firstLoad = false;

            // Load data from json file
            if (JsonSaveLoadUtility.TryLoad(PLAYER_DATA_SAVE_PATH, out PlayerData playerData))
            {
                _playerDataBuffer.PlayerData = playerData;
            }

            // Load texture from saved image
            if (_avatarPickerService.TryLoadSavedAvatar())
            {
                playerData.PlayerAvatar = _avatarPickerService.GetAvatarBytes();
                _playerDataBuffer.ChangedData |= ChangedData.Avatar;
            }
        }


        public void Initialize()
        {
        }


        ~PlayerConfigurationService()
        {
            _eventBus.Unsubscribe<S_OnPlayerConfigurationSaveRequested>(UpdatePlayer);
            _eventBus.Unsubscribe<S_OnAvatarPickRequested>(PickAvatar);
            _eventBus.Unsubscribe<S_OnPlayerDataRequested>(OnDataRequested);
        }
    }
}
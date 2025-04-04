using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase
{
    public class ConnectionUIManager : MonoBehaviour
    {
        [SerializeField] private UITemplate _joinRoomUI;
        [SerializeField] private UITemplate _roomUI;
        [SerializeField] private UITemplate _searchRoomUI;
        [SerializeField] private UITemplate _playerConfigurationUI;

        private void Awake()
        {
            _joinRoomUI.Show();
            _roomUI.Hide();
            _searchRoomUI.Hide();
            _playerConfigurationUI.Hide();
        }
    }
}
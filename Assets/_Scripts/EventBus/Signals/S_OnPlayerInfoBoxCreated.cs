using EBus;
using UnityEngine;

namespace CodeBase.EventBusSignals
{
    public sealed class S_OnPlayerInfoBoxCreated : IDebugableSignal
    {
        public readonly ulong PlayerId;
        public readonly PlayerInfoBoxUI PlayerInfoBoxUI;


        public S_OnPlayerInfoBoxCreated(ulong playerId, PlayerInfoBoxUI playerInfoBoxUI)
        {
            PlayerId = playerId;
            PlayerInfoBoxUI = playerInfoBoxUI;
        }


        public string DebugMessage()
        {
            return $"Player Id: {PlayerId}  UI Info Box: {PlayerInfoBoxUI.gameObject.name}";
        }
    }
}
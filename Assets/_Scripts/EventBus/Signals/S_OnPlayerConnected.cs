using EBus;
using UnityEngine;

namespace CodeBase
{
    public class S_OnPlayerConnected : IDebugableSignal
    {
        public readonly ulong ConnectedPlayerId;
        public readonly bool ConnectedSelf;


        public S_OnPlayerConnected(ulong connectedPlayerId, bool connectedSelf)
        {
            ConnectedPlayerId = connectedPlayerId;
            ConnectedSelf = connectedSelf;
        }


        public string DebugMessage()
        {
            return $"Player Id: {ConnectedPlayerId} Disconnected Self: {ConnectedSelf}";
        }
    }
}
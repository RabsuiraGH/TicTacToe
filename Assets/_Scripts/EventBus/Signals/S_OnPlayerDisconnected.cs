using EBus;

namespace CodeBase.EventBusSignals
{
    public class S_OnPlayerDisconnected : IDebugableSignal
    {
        public readonly ulong DisconnectedPlayerId;
        public readonly bool DisconnectedSelf;


        public S_OnPlayerDisconnected(ulong disconnectedPlayerId, bool disconnectedSelf)
        {
            DisconnectedPlayerId = disconnectedPlayerId;
            DisconnectedSelf = disconnectedSelf;
        }


        public string DebugMessage()
        {
            return $"Player Id: {DisconnectedPlayerId} Disconnected Self: {DisconnectedSelf}";
        }
    }
}
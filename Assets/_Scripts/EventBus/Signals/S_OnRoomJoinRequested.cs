using EBus;

namespace CodeBase.EventBusSignals
{
    public sealed class S_OnRoomJoinRequested : IDebugableSignal
    {
        public readonly string RoomId;


        public S_OnRoomJoinRequested(string roomId)
        {
            RoomId = roomId;
        }


        public string DebugMessage()
        {
            return $"Room IP: {RoomId}";
        }
    }
}
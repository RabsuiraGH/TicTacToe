using EBus;

namespace CodeBase.EventBusSignals
{
    public sealed class S_OnRoomCreationRequested : IDebugableSignal
    {
        public readonly string RoomId;


        public S_OnRoomCreationRequested(string roomId)
        {
            RoomId = roomId;
        }


        public string DebugMessage()
        {
            return $"Room IP: {RoomId}";
        }
    }
}
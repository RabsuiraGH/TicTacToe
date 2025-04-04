using EBus;

namespace CodeBase.EventBusSignals
{
    public sealed class S_OnRoomCreationStarted : IDebugableSignal
    {
        public readonly bool Succeed;
        public readonly string RoomIP;


        public S_OnRoomCreationStarted(bool succeed, string roomIP = "")
        {
            Succeed = succeed;
            RoomIP = roomIP;
        }

        public string DebugMessage()
        {
            return $"Succeed: {Succeed}, Room IP: {RoomIP}";
        }
    }
}
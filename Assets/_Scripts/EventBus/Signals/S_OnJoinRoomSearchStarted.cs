using EBus;

namespace CodeBase.EventBus.Signals
{
    public sealed class S_OnJoinRoomSearchStarted : IDebugableSignal
    {
        public readonly bool Succeed;
        public readonly string RoomIP;


        public S_OnJoinRoomSearchStarted(bool succeed, string roomIP = "")
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
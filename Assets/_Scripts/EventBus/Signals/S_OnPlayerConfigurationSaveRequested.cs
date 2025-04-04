using EBus;

namespace CodeBase.EventBusSignals
{
    public sealed class S_OnPlayerConfigurationSaveRequested : IDebugableSignal
    {
        public readonly string PlayerName;
        public S_OnPlayerConfigurationSaveRequested(string playerName)
        {
            PlayerName = playerName;
        }


        public string DebugMessage()
        {
            return $"New Player Name: {PlayerName}";
        }
    }
}
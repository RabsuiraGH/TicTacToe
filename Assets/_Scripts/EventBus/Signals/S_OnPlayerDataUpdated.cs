namespace CodeBase.EventBusSignals
{
    public sealed class S_OnPlayerDataUpdated
    {
        public readonly PlayerData PlayerData;


        public S_OnPlayerDataUpdated(PlayerData playerData)
        {
            PlayerData = playerData;
        }
    }
}
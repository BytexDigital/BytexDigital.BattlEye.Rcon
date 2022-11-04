namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to set the maximum acceptable ping for players.
    /// </summary>
    public class SetMaxPingCommand : Command
    {
        /// <summary>
        ///     Requests the server to set the maximum acceptable ping for players.
        /// </summary>
        /// <param name="maxPing">New maximum ping</param>
        public SetMaxPingCommand(int maxPing) : base($"MaxPing {maxPing}")
        {
        }
    }
}
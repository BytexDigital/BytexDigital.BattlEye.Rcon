namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to restart the currently running mission.
    /// </summary>
    public class RestartMissionCommand : Command
    {
        /// <summary>
        ///     Requests the server to restart the currently running mission.
        /// </summary>
        public RestartMissionCommand() : base("#restart")
        {
        }
    }
}
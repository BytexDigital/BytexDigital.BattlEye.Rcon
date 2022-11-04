namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to load a mission.
    /// </summary>
    public class LoadMissionCommand : Command
    {
        /// <summary>
        ///     Requests the server to load the specified mission.
        /// </summary>
        /// <param name="missionName">Name of mission to load</param>
        public LoadMissionCommand(string missionName) : base($"#mission {missionName}")
        {
        }
    }
}
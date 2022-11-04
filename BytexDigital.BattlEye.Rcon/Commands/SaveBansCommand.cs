namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to write all currently loaded and in memory kept bans to be written to the bans.txt.
    /// </summary>
    public class SaveBansCommand : Command
    {
        /// <summary>
        ///     Requests the server to write all currently loaded and in memory kept bans to be written to the bans.txt.
        /// </summary>
        public SaveBansCommand() : base("writeBans")
        {
        }
    }
}
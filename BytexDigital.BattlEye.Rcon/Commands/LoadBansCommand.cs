namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to reload all bans from the bans.txt.
    /// </summary>
    public class LoadBansCommand : Command
    {
        /// <summary>
        ///     Requests the server to reload all bans from the bans.txt.
        /// </summary>
        public LoadBansCommand() : base("loadBans")
        {
        }
    }
}
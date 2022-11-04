namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to remove a ban.
    /// </summary>
    public class RemoveBanCommand : Command
    {
        /// <summary>
        ///     Requests the server remove the ban with the specified id.
        /// </summary>
        /// <param name="banId">ID of ban to remove</param>
        public RemoveBanCommand(int banId) : base($"removeBan {banId}")
        {
        }
    }
}
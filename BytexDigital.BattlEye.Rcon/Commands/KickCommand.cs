namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to remove a player from the server.
    /// </summary>
    public class KickCommand : Command
    {
        /// <summary>
        ///     Requests the server to remove the given player ID from the server.
        /// </summary>
        /// <param name="playerId"></param>
        public KickCommand(int playerId) : base($"kick {playerId}")
        {
        }

        /// <summary>
        ///     Requests the server to remove the given player ID from the server with the specified reason.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="reason"></param>
        public KickCommand(int playerId, string reason) : base($"kick {playerId} {reason}")
        {
        }
    }
}
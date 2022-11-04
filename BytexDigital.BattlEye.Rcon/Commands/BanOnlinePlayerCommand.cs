using System;

namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to ban a player that is currently on the server.
    /// </summary>
    public class BanOnlinePlayerCommand : Command
    {
        /// <summary>
        ///     Requests the server to ban the specified player that is currently on the server.
        /// </summary>
        /// <param name="guid">GUID of the player to ban</param>
        /// <param name="reason">Reason to ban the player with</param>
        /// <param name="duration">Duration of ban</param>
        public BanOnlinePlayerCommand(string guid, string reason, TimeSpan duration) : base(
            $"ban {guid} {duration.TotalMinutes.ToString("0")} {reason}")
        {
        }

        /// <summary>
        ///     Requests the server to ban the specified player permanently that is currently on the server.
        /// </summary>
        /// <param name="guid">GUID of the player to ban</param>
        /// <param name="reason">Duration of ban</param>
        public BanOnlinePlayerCommand(string guid, string reason) : base($"ban {guid} -1 {reason}")
        {
        }
    }
}
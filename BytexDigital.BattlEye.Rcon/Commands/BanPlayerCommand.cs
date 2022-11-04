using System;

namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests to ban a player regardless of whether he is currently on the server or not.
    /// </summary>
    public class BanPlayerCommand : Command
    {
        /// <summary>
        ///     Requests to ban the specified player regardless of whether he is currently on the server or not.
        /// </summary>
        /// <param name="guid">GUID of player to ban</param>
        /// <param name="reason">Reason to ban the player with</param>
        /// <param name="duration">Duration of ban</param>
        public BanPlayerCommand(string guid, string reason, TimeSpan duration) : base(
            $"addBan {guid} {duration.TotalMinutes.ToString("0")} {reason}")
        {
        }

        /// <summary>
        ///     Requests to ban the specified player permanently regardless of whether he is currently on the server or not.
        /// </summary>
        /// <param name="guid">GUID of player to ban</param>
        /// <param name="reason">Reason to ban the player with</param>
        public BanPlayerCommand(string guid, string reason) : base($"addBan {guid} -1 {reason}")
        {
        }
    }
}
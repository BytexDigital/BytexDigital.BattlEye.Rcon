using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to ban a player that is currently on the server. Provides an overload for bans with a timeframe and permanent bans.
    /// </summary>
    public class BanOnlinePlayerCommand : Command {
        public BanOnlinePlayerCommand(string guid, string reason, TimeSpan duration) : base($"ban {guid} {duration.TotalMinutes.ToString("0")} {reason}") {
        }

        public BanOnlinePlayerCommand(string guid, string reason) : base($"ban {guid} -1 {reason}") {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests to ban a player regardless of whether he is currently on the server or not. Provides an overload for bans with a timeframe and permanent bans.
    /// </summary>
    public class BanPlayerCommand : Command {
        public BanPlayerCommand(string guid, string reason, TimeSpan duration) : base($"addBan {guid} {duration.TotalMinutes.ToString("0")} {reason}") {
        }

        public BanPlayerCommand(string guid, string reason) : base($"addBan {guid} -1 {reason}") {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class BanOnlinePlayerCommand : Command {
        public BanOnlinePlayerCommand(string guid, string reason, TimeSpan duration) : base($"ban {guid} {duration.TotalMinutes.ToString("0")} {reason}") {
        }

        public BanOnlinePlayerCommand(string guid, string reason) : base($"ban {guid} -1 {reason}") {
        }
    }
}

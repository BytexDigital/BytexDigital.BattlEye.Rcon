using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class BanPlayerCommand : Command {
        public BanPlayerCommand(string guid, string reason, TimeSpan duration) : base($"addBan {guid} {duration.TotalMinutes.ToString("0")} {reason}") {
        }

        public BanPlayerCommand(string guid, string reason) : base($"addBan {guid} -1 {reason}") {
        }
    }
}

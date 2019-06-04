using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to set the maximum acceptable ping for players.
    /// </summary>
    public class SetMaxPingCommand : Command {
        public SetMaxPingCommand(int maxPing) : base($"MaxPing {maxPing}") {
        }
    }
}

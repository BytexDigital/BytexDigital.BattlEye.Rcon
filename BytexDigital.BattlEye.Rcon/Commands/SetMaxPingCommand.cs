using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class SetMaxPingCommand : Command {
        public SetMaxPingCommand(int maxPing) : base($"MaxPing {maxPing}") {
        }
    }
}

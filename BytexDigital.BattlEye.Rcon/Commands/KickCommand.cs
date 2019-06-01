using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class KickCommand : Command {
        public KickCommand(int playerId) : base($"kick {playerId}") {
        }
    }
}

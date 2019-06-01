using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class RemoveBanCommand : Command {
        public RemoveBanCommand(int banId) : base($"removeBan {banId}") {
        }
    }
}

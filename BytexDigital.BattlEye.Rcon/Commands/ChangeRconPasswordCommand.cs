using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class ChangeRconPasswordCommand : Command {
        public ChangeRconPasswordCommand(string newPassword) : base($"RConPassword {newPassword}") {
        }
    }
}

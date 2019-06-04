using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to change the rcon password immediately. Currently authenticated <see cref="RconClient"/>s will stay authenticated.
    /// </summary>
    public class ChangeRconPasswordCommand : Command {
        public ChangeRconPasswordCommand(string newPassword) : base($"RConPassword {newPassword}") {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to remove ban.
    /// </summary>
    public class RemoveBanCommand : Command {
        public RemoveBanCommand(int banId) : base($"removeBan {banId}") {
        }
    }
}

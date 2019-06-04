using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to reload all bans from the bans.txt.
    /// </summary>
    public class LoadBansCommand : Command {
        public LoadBansCommand() : base("loadBans") {
        }
    }
}

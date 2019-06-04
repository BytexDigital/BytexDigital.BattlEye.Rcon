using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to write all currently loaded and in memory kept bans to be written to the bans.txt.
    /// </summary>
    public class SaveBansCommand : Command {
        public SaveBansCommand() : base("writeBans") {
        }
    }
}

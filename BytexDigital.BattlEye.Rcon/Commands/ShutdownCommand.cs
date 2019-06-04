using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to shut down.
    /// </summary>
    public class ShutdownCommand : Command {
        public ShutdownCommand() : base("#shutdown") {
        }
    }
}

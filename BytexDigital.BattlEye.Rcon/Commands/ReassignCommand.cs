using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to throw all players back to the assignment screen.
    /// </summary>
    public class ReassignCommand : Command {
        public ReassignCommand() : base("#reassign") {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to unlock itself so new players can join.
    /// </summary>
    public class UnlockCommand : Command {
        public UnlockCommand() : base("#unlock") {
        }
    }
}

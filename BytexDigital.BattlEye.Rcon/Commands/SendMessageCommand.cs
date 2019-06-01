using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class SendMessageCommand : Command {
        public SendMessageCommand(string message) : base($"say -1 {message}") {
        }

        public SendMessageCommand(int playerId, string message) : base($"say {playerId} {message}") {
        }
    }
}

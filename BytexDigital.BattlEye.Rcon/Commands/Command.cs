using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public abstract class Command {
        public string Content { get; private set; }

        public Command(string content) {
            Content = content;
        }
    }
}

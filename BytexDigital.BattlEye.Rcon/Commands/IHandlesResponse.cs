using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public interface IHandlesResponse {
        void Handle(string content);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class LoadMissionCommand : Command {
        public LoadMissionCommand(string missionName) : base($"#mission {missionName}") {
        }
    }
}

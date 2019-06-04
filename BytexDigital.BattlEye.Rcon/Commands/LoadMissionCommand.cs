using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the server to load the specified mission.
    /// </summary>
    public class LoadMissionCommand : Command {
        public LoadMissionCommand(string missionName) : base($"#mission {missionName}") {
        }
    }
}

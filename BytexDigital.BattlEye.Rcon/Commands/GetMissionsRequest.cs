using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    public class GetMissionsRequest : Command, IHandlesResponse {
        public GetMissionsRequest() : base("missions") {
        }

        public void Handle(string content) {
            
        }
    }
}

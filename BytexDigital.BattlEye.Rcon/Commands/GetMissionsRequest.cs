using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Commands {
    /// <summary>
    /// Requests the currently available missions on the server. Appears to be broken.
    /// </summary>
    public class GetMissionsRequest : Command, IHandlesResponse {
        /// <summary>
        /// Requests the currently available missions on the server. Appears to be broken.
        /// </summary>
        public GetMissionsRequest() : base("missions") {
        }

        public void Handle(string content) {
            
        }
    }
}

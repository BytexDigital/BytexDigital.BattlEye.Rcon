using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Responses {
    public class LoginNetworkResponse : NetworkResponse {
        public bool Success { get; private set; }

        public LoginNetworkResponse(bool success) {
            Success = success;
        }
    }
}

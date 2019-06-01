using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Requests {
    public class LoginNetworkRequest : NetworkRequest {
        public string Password { get; private set; }

        public LoginNetworkRequest(string password) {
            Password = password;
        }

        internal override byte[] GetPayloadBytes() {
            var bytes = new List<byte>();

            bytes.Add(0x00);
            bytes.AddRange(Encoding.ASCII.GetBytes(Password));

            return bytes.ToArray();
        }
    }
}

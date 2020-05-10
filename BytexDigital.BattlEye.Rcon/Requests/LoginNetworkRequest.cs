using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Requests
{
    public class LoginNetworkRequest : NetworkRequest
    {
        public string Password { get; private set; }

        private readonly StringEncoder _stringEncoder = new StringEncoder();

        public LoginNetworkRequest(string password)
        {
            Password = password;
        }

        internal override byte[] GetPayloadBytes()
        {
            var bytes = new List<byte>();

            bytes.Add(0x00);
            bytes.AddRange(_stringEncoder.GetBytes(Password));

            return bytes.ToArray();
        }
    }
}

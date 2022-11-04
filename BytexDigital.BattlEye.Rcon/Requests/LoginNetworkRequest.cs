using System.Collections.Generic;

namespace BytexDigital.BattlEye.Rcon.Requests
{
    public class LoginNetworkRequest : NetworkRequest
    {
        private readonly StringEncoder _stringEncoder = new StringEncoder();
        public string Password { get; }

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
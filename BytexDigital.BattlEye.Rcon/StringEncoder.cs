using System.Text;

namespace BytexDigital.BattlEye.Rcon
{
    public class StringEncoder
    {
        private readonly Encoding _encoding = Encoding.GetEncoding(1252);

        public byte[] GetBytes(string message) => _encoding.GetBytes(message);
        public string GetString(byte[] bytes) => _encoding.GetString(bytes);
    }
}

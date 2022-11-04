using System.Text;

namespace BytexDigital.BattlEye.Rcon
{
    public class StringEncoder
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        public byte[] GetBytes(string message)
        {
            return _encoding.GetBytes(message);
        }

        public string GetString(byte[] bytes)
        {
            return _encoding.GetString(bytes);
        }
    }
}
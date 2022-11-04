using System.Net;

namespace BytexDigital.BattlEye.Rcon.Domain
{
    public class Player
    {
        public int Id { get; }
        public IPEndPoint RemoteEndpoint { get; }
        public int Ping { get; }
        public string Guid { get; }
        public string Name { get; }
        public bool IsVerified { get; }
        public bool IsInLobby { get; }

        public Player(
            int id,
            IPEndPoint remoteEndpoint,
            int ping,
            string guid,
            string name,
            bool isVerified,
            bool isInLobby)
        {
            Id = id;
            RemoteEndpoint = remoteEndpoint;
            Ping = ping;
            Guid = guid;
            Name = name;
            IsVerified = isVerified;
            IsInLobby = isInLobby;
        }
    }
}
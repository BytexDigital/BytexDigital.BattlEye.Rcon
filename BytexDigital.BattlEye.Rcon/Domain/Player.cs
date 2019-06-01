using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Domain {
    public class Player {
        public int Id { get; private set; }
        public IPEndPoint RemoteEndpoint { get; private set; }
        public int Ping { get; private set; }
        public string Guid { get; private set; }
        public string Name { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsInLobby { get; private set; }

        public Player(int id, IPEndPoint remoteEndpoint, int ping, string guid, string name, bool isVerified, bool isInLobby) {
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

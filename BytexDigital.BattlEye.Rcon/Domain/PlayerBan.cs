using System;
using System.Net;
using System.Threading;

namespace BytexDigital.BattlEye.Rcon.Domain
{
    public class PlayerBan
    {
        public int Id { get; }
        public string Guid { get; }
        public IPAddress Ip { get; }
        public TimeSpan DurationLeft { get; }
        public bool IsPermanent => DurationLeft == Timeout.InfiniteTimeSpan;
        public string Reason { get; }
        public bool IsIpBan => Ip != null;
        public bool IsGuidBan => !string.IsNullOrEmpty(Guid);

        public PlayerBan(int banId, string guid, IPAddress ip, TimeSpan duration, string reason)
        {
            Id = banId;
            Guid = guid;
            Ip = ip;
            DurationLeft = duration;
            Reason = reason;
        }
    }
}
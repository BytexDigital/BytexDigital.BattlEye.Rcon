using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace BytexDigital.BattlEye.Rcon.Domain
{
    public class PlayerBan
    {

        public int Id { get; private set; }
        public string Guid { get; private set; }
        public IPAddress Ip { get; private set; }
        public TimeSpan DurationLeft { get; private set; }
        public bool IsPermanent { get { return DurationLeft == Timeout.InfiniteTimeSpan; } }
        public string Reason { get; private set; }
        public bool IsIpBan { get { return Ip != null; } }
        public bool IsGuidBan { get { return !string.IsNullOrEmpty(Guid); } }

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

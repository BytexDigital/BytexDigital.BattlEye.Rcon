using System;

namespace BytexDigital.BattlEye.Rcon.Events
{
    public class PlayerRemovedArgs : EventArgs
    {
        public int Id { get; }
        public string Guid { get; }
        public string Name { get; }
        public bool IsBan { get; }
        public string Reason { get; }

        public PlayerRemovedArgs(int id, string guid, string name, bool isBan, string reason)
        {
            Id = id;
            Guid = guid;
            Name = name;
            IsBan = isBan;
            Reason = reason;
        }
    }
}
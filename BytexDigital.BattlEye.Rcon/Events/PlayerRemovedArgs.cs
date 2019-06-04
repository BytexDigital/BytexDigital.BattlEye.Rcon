using System;

namespace BytexDigital.BattlEye.Rcon.Events {
    public class PlayerRemovedArgs : EventArgs {
        public int Id { get; private set; }
        public string Guid { get; private set; }
        public string Name { get; private set; }
        public bool IsBan { get; private set; }
        public string Reason { get; private set; }

        public PlayerRemovedArgs(int id, string guid, string name, bool isBan, string reason) {
            Id = id;
            Guid = guid;
            Name = name;
            IsBan = isBan;
            Reason = reason;
        }
    }
}

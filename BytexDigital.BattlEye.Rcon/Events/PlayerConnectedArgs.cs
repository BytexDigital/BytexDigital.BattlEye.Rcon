using System;

namespace BytexDigital.BattlEye.Rcon.Events {
    public class PlayerConnectedArgs : EventArgs {
        public int Id { get; private set; }
        public string Guid { get; private set; }
        public string Name { get; private set; }

        public PlayerConnectedArgs(int id, string guid, string name) {
            Id = id;
            Guid = guid;
            Name = name;
        }
    }
}

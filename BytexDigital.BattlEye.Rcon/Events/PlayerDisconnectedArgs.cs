using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Events {
    public class PlayerDisconnectedArgs {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public PlayerDisconnectedArgs(int id, string name) {
            Id = id;
            Name = name;
        }
    }
}

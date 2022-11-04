using System;

namespace BytexDigital.BattlEye.Rcon.Events
{
    public class PlayerDisconnectedArgs : EventArgs
    {
        public int Id { get; }
        public string Name { get; }

        public PlayerDisconnectedArgs(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
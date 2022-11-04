using System;

namespace BytexDigital.BattlEye.Rcon.Events
{
    public class PlayerConnectedArgs : EventArgs
    {
        public int Id { get; }
        public string Guid { get; }
        public string Name { get; }

        public PlayerConnectedArgs(int id, string guid, string name)
        {
            Id = id;
            Guid = guid;
            Name = name;
        }
    }
}
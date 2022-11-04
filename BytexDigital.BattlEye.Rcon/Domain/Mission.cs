namespace BytexDigital.BattlEye.Rcon.Domain
{
    public class Mission
    {
        public string Name { get; }
        public string Map { get; }

        public Mission(string name, string map)
        {
            Name = name;
            Map = map;
        }
    }
}
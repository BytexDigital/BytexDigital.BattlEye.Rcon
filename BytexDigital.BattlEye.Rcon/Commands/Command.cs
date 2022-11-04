namespace BytexDigital.BattlEye.Rcon.Commands
{
    public abstract class Command
    {
        public string Content { get; }

        public Command(string content)
        {
            Content = content;
        }
    }
}
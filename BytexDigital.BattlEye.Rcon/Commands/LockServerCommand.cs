namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to lock itself so new players can't join.
    /// </summary>
    public class LockServerCommand : Command
    {
        /// <summary>
        ///     Requests the server to lock itself so new players can't join.
        /// </summary>
        public LockServerCommand() : base("#lock")
        {
        }
    }
}
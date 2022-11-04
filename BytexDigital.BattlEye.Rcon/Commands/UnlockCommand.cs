namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to unlock itself so new players can join.
    /// </summary>
    public class UnlockCommand : Command
    {
        /// <summary>
        ///     Requests the server to unlock itself so new players can join.
        /// </summary>
        public UnlockCommand() : base("#unlock")
        {
        }
    }
}
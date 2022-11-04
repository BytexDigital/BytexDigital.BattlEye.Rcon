namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to change the rcon password immediately. Currently authenticated <see cref="RconClient" />s
    ///     will stay authenticated.
    /// </summary>
    public class ChangeRconPasswordCommand : Command
    {
        /// <summary>
        ///     Requests the server to change the rcon password to the specified one immediately. Currently authenticated
        ///     <see cref="RconClient" />s will stay authenticated.
        /// </summary>
        /// <param name="newPassword">New rcon password</param>
        public ChangeRconPasswordCommand(string newPassword) : base($"RConPassword {newPassword}")
        {
        }
    }
}
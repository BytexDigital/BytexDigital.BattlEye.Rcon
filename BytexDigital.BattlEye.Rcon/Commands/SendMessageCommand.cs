namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to broadcast a message.
    /// </summary>
    public class SendMessageCommand : Command
    {
        /// <summary>
        ///     Requests the server to broadcast a message to everyone.
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public SendMessageCommand(string message) : base($"say -1 {message}")
        {
        }

        /// <summary>
        ///     Requests the server to broadcast a message to a specific player.
        /// </summary>
        /// <param name="playerId">ID to player to broadcast to</param>
        /// <param name="message">Message to broadcast</param>
        public SendMessageCommand(int playerId, string message) : base($"say {playerId} {message}")
        {
        }
    }
}
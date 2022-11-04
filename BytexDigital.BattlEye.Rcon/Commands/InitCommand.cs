namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to reload it's config file. In Arma and DayZ, this is the config file configured in the -config
    ///     parameter.
    /// </summary>
    public class InitCommand : Command
    {
        /// <summary>
        ///     Requests the server to reload it's config file. In Arma and DayZ, this is the config file configured in the -config
        ///     parameter.
        /// </summary>
        public InitCommand() : base("#init")
        {
        }
    }
}
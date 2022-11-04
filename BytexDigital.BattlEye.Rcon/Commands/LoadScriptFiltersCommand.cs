namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to reload the script execution filters specified in scripts.txt.
    /// </summary>
    public class LoadScriptFiltersCommand : Command
    {
        /// <summary>
        ///     Requests the server to reload the script execution filters specified in scripts.txt.
        /// </summary>
        public LoadScriptFiltersCommand() : base("loadScripts")
        {
        }
    }
}
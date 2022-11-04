namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Requests the server to reload all network filters such as createVehicle.txt, remoteExec.txt and so on.
    /// </summary>
    public class LoadEventFiltersCommand : Command
    {
        /// <summary>
        ///     Requests the server to reload all network filters such as createVehicle.txt, remoteExec.txt and so on.
        /// </summary>
        public LoadEventFiltersCommand() : base("loadEvents")
        {
        }
    }
}
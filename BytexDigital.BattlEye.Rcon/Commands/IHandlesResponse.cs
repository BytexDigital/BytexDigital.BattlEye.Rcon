namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Marks that the given class handles the returned string response.
    /// </summary>
    public interface IHandlesResponse
    {
        void Handle(string content);
    }
}
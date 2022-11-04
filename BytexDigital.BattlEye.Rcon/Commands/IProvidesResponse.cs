namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    ///     Marks that the given class exposes a response of type.
    /// </summary>
    /// <typeparam name="T">Datatype of response.</typeparam>
    public interface IProvidesResponse<T>
    {
        T GetResponse();
    }
}
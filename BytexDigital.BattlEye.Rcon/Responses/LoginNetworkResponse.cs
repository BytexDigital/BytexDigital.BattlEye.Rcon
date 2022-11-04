namespace BytexDigital.BattlEye.Rcon.Responses
{
    public class LoginNetworkResponse : NetworkResponse
    {
        public bool Success { get; }

        public LoginNetworkResponse(bool success)
        {
            Success = success;
        }
    }
}
namespace BytexDigital.BattlEye.Rcon.Requests
{
    public abstract class SequentialNetworkRequest : NetworkRequest
    {
        public byte? SequenceNumber { get; protected set; }

        internal void SetSequenceNumber(byte number)
        {
            SequenceNumber = number;
        }
    }
}
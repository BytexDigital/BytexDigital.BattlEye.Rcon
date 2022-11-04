namespace BytexDigital.BattlEye.Rcon
{
    public class SequenceCounter
    {
        public byte Value { get; private set; }

        public byte Next()
        {
            return Value++;
        }
    }
}
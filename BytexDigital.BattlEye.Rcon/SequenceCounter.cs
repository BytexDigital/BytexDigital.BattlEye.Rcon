namespace BytexDigital.BattlEye.Rcon {
    public class SequenceCounter {
        public byte Value { get; private set; }

        public SequenceCounter() : this(0) { }

        public SequenceCounter(int initialValue) {

        }

        public byte Next() {
            if (Value >= 255) {
                Value = 0;
            }

            return Value++;
        }
    }
}

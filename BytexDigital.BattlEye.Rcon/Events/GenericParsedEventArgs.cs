using System;

namespace BytexDigital.BattlEye.Rcon.Events {
    public class GenericParsedEventArgs : EventArgs {
        public object Arguments { get; private set; }

        public GenericParsedEventArgs(EventArgs arguments) {
            Arguments = arguments;
        }
    }
}

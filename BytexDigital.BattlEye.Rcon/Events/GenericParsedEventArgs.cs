using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Events {
    public class GenericParsedEventArgs {
        public object Arguments { get; private set; }

        public GenericParsedEventArgs(object arguments) {
            Arguments = arguments;
        }
    }
}

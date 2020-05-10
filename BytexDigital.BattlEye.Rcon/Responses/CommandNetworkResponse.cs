using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon.Responses
{
    public class CommandNetworkResponse : NetworkResponse
    {
        public string Content { get; private set; }

        public CommandNetworkResponse(string content)
        {
            Content = content;
        }

        internal void AppendContent(string content)
        {
            Content += content;
        }
    }
}

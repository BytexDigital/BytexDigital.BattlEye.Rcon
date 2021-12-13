﻿using System;
using System.Collections.Generic;

namespace BytexDigital.BattlEye.Rcon.Responses
{
    public class CommandNetworkResponse : NetworkResponse
    {
        public string Content { get; private set; }

        private readonly List<byte> _data = new List<byte>();

        public CommandNetworkResponse(string content)
        {
            Content = content;
        }

        internal void AppendContentBytes(IEnumerable<byte> data)
        {
            _data.AddRange(data);
        }

        internal void ConvertCollectedBytesToContentString(Func<List<byte>, string> converter)
        {
            Content = converter.Invoke(_data);
        }
    }
}

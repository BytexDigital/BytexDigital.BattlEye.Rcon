﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BytexDigital.BattlEye.Rcon
{
    public enum NetworkPackageType : byte
    {
        Login = 0x00,
        Command = 0x01,
        Message = 0x02
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainReact.Core.Server
{
    public enum CommandProtocol : byte
    {
        Unknown = 0,

        Request = 1,

        MapRequest = 2,

        MapData = 3,

        PlayerData = 4,

        SetData = 5,

        GameOverData = 6,

        Error = 7,

        ExplodingWabe = 8,

        Ready = 9,

        Restarting = 10
    }
}

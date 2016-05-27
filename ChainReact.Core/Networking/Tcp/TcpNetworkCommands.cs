using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainReact.Core.Networking.Tcp
{
    public enum TcpNetworkCommands : byte
    {
        ClientConnect = 1,
        ClientInformation = 2,
        ServerReady = 3,
        MessageUpdate = 4,
        ClientWabeSet = 5,
        GameMapHash = 6,
        MapRequest = 7,
        MapData = 8,
        ConnectionClosing = 9,
        ResetGame = 10,
        CurrentPlayerChanged = 11,
        PlayerLeft = 12,
        Unknown = 255
    }
}

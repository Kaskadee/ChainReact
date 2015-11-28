using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using ChainReact.Core.Game.Objects;
using Sharpex2D.Framework.Network;

namespace ChainReact.Core.Server
{
    public sealed class Client
    {
        public IPEndPoint Peer { get; set; }
        public bool IsBot { get; set; }
        public Player Player { get; set; }

        public Client(IPEndPoint peer, Player p, bool isBot = false)
        {
            Peer = peer;
            Player = p;
            IsBot = isBot;
        }
    }
}

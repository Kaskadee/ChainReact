using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChainReact.Networking.Events.Udp
{
    public class UdpClientJoinedEventArgs : EventArgs
    {
        public UdpGameClient Client { get; set; }

        public UdpClientJoinedEventArgs(UdpGameClient client)
        {
            Client = client;
        }
    }
}

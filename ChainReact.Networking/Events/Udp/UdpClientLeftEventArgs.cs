using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainReact.Networking.Events.Udp
{
    public class UdpClientLeftEventArgs
    {
        public UdpGameClient Client { get; set; }

        public UdpClientLeftEventArgs(UdpGameClient client)
        {
            Client = client;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChainReact.Networking
{
    public class UdpGameClient
    {
        public IPEndPoint RemoteEndPoint { get; private set; }
        public object Tag { get; set; }

        public bool IsAlive { get; private set; }

        public UdpGameClient(IPEndPoint point, object tag = null)
        {
            RemoteEndPoint = point;
            Tag = tag;
        }

        public void SetAliveState(bool state)
        {
            IsAlive = state;
        }
    }
}

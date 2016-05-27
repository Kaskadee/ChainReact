using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainReact.Core.Networking.Tcp.Events
{
    public class GameConnectionFailedEventArgs : EventArgs
    {
        public string Reason { get; private set; }
        public Exception Exception { get; private set; }

        public GameConnectionFailedEventArgs(string reason, Exception e)
        {
            Reason = reason;
            Exception = e;
        }
    }
}

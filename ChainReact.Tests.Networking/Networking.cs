using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpex2D.Framework.Network;

namespace ChainReact.Tests.Networking
{
    [TestClass]
    public class Networking
    {
        [TestMethod]
        public void TestMethod1() {
            //var server = new UdpBroadcastServer(8889);
            //var client = new UdpBroadcastClient(8888, 8889);
            
            //client.Send("Hello");
            //Console.ReadLine();
        }

        [TestMethod]
        public void NetworkAddresses()
        {
            var items = NetworkInterface.GetAllNetworkInterfaces().Where(adapter => !adapter.Name.Contains("VM") && !adapter.Name.Contains("VirtualBox"))
    .SelectMany(adapter => adapter.GetIPProperties().UnicastAddresses)
    .Where(adr => adr.Address.AddressFamily == AddressFamily.InterNetwork && adr.IsDnsEligible)
    .Select(adr => adr.Address.ToString());
            foreach (var item in items)
            {
                Debug.WriteLine(item);
            }
        }
    }
}

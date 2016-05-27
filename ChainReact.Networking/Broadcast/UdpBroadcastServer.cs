using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChainReact.Networking.Events.Udp;

namespace ChainReact.Networking.Broadcast
{
    public class UdpBroadcastServer
    {
        public const List<> 

        public IPEndPoint LocalEndPoint { get; }

        public bool Started => _client?.Client.IsBound ?? false;

        public List<UdpGameClient> ConnectedClients { get; } = new List<UdpGameClient>(); 
        private readonly UdpClient _client;

        private bool _allowReceiving;

        public event EventHandler<UdpClientJoinedEventArgs> ClientJoined;
        public event EventHandler<UdpReceivedEventArgs> MessageReceived;
        public event EventHandler<UdpClientLeftEventArgs> ClientDisconnected;

        public UdpBroadcastServer(int port)
        {
            LocalEndPoint = new IPEndPoint(IPAddress.Any, port);
            _client = new UdpClient();
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            _client.Client.ExclusiveAddressUse = false;
        }

        public void Start()
        {
            if(_client.Client.IsBound)
                throw new InvalidOperationException("The udp server already started.");
            _client.Client.Bind(LocalEndPoint);
            _allowReceiving = true;
            Receive();
        }

        public void Stop()
        {
            _allowReceiving = false;
            _client.Close();
        }

        public void SendAll(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            foreach (var client in ConnectedClients)
            {
                Send(client, data);
            }
        }

        public void Send(IPEndPoint endPoint, string message)
        {
            Send(endPoint, Encoding.UTF8.GetBytes(message));
        }

        public void Send(IPEndPoint endPoint, byte[] message)
        {
            _client.BeginSend(message, message.Length, endPoint, SendCallback, null);
        }

        private void SendCallback(IAsyncResult result)
        {
            _client.EndSend(result);
        }

        public void Receive()
        {
            if (!_allowReceiving)
                return;
            _client.BeginReceive(ReceiveCallback, null);
        }

        public void ReceiveCallback(IAsyncResult result)
        {
            var endPoint = LocalEndPoint;
            var bytes = _client.EndReceive(result, ref endPoint);
            MessageReceived?.Invoke(this, new UdpReceivedEventArgs(bytes));
        }
    }
}

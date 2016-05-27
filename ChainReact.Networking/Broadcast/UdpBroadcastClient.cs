using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChainReact.Networking.Broadcast
{
    public class UdpBroadcastClient
    {
        public IPEndPoint LocalEndPoint { get; }

        public int ListeningPort { get; }
        public int SendingPort { get; }

        public event EventHandler<UdpReceivedEventArgs> OnMessageReceived;

        private readonly UdpGameClient _client;

        public UdpBroadcastClient(int listeningPort, int sendingPort)
        {
            ListeningPort = listeningPort;
            SendingPort = sendingPort;
            var broadcastAddress = new IPEndPoint(IPAddress.Any, listeningPort);
            LocalEndPoint = broadcastAddress;
            _client = new UdpGameClient();
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            _client.Client.ExclusiveAddressUse = false;
            Receive();
        }

        public void Send(string message)
        {
            Send(Encoding.UTF8.GetBytes(message));
        }

        public void Send(byte[] message)
        {
            if(!_client.Client.IsBound)
                _client.Client.Bind(LocalEndPoint);
            _client.BeginSend(message, message.Length, new IPEndPoint(IPAddress.Broadcast, SendingPort),
                SendCallback, null);
        }

        public void Receive()
        {
            if (!_client.Client.IsBound)
                _client.Client.Bind(LocalEndPoint);
            _client.BeginReceive(ReceiveCallback, null);
        }

        private void SendCallback(IAsyncResult result)
        {
            _client.EndSend(result);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            var endPoint = LocalEndPoint;
            var bytes = _client.EndReceive(result, ref endPoint);
            var eventArgs = new UdpReceivedEventArgs(bytes);
            OnMessageReceived?.Invoke(this, eventArgs);
            Receive();
        }
    }

    public class UdpReceivedEventArgs : EventArgs
    {
        public byte[] Bytes { get; }

        public UdpReceivedEventArgs(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}

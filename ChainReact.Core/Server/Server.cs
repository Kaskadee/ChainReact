using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework.Network;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Server
{
    public class Server : IDisposable
    {
        private bool _disposed;

        private readonly NetworkPeer _server;
        private readonly List<Client> _clients;
        private readonly int _maxSlots;
        private readonly List<Player> _unusedPlayers;
        private ChainReactGame _game;

        public ChainReactGame RemoteGame => _game;

        private Dictionary<string, Action<RemotePeer, string>> _commands = new Dictionary<string, Action<RemotePeer, string>>();

        public Server(ServerMode mode, NetworkPeer.Protocol protocol, int port = 22794)
        {
            _unusedPlayers = new List<Player>
                {
                    new Player(1, "Player1", (() => Color.Green)) {Enabled = true},
                    new Player(2, "Player2", (() => Color.Red)) {Enabled = true},
                    new Player(3, "Player3", (() => Color.Blue)),
                    new Player(4, "Player4", (() => Color.Orange))
                };
            _clients = new List<Client>();
            IPAddress point;
            switch (mode)
            {
                case ServerMode.Internal:
                    point = IPAddress.Loopback;
                    _maxSlots = 1;
                    break;
                case ServerMode.Global:
                    point = IPAddress.Any;
                    _maxSlots = 4;
                    break;
                default:
                    point = IPAddress.None;
                    _maxSlots = 0;
                    break;

            }
            var defaultCommands = new ServerCommands(this, _commands);
            _commands = defaultCommands.RegisterCommands();
            _server = new NetworkPeer(protocol, point, port) { MaxReceiveBuffer = 1000000 };
            _server.PeerJoined += PlayerJoined;
            _server.MessageArrived += MessageReceived;
            _server.PeerDisconnected += PlayerDisconnected;
            _game = new ChainReactGame();
        }

        public void Shutdown()
        {
            Dispose();
        }

        private void PlayerJoined(object sender, PeerJoinedEventArgs e)
        {
            _server.Send(new OutgoingMessage(Encoding.UTF8.GetBytes("ready")), e.RemotePeer);
            if (_server.Connections.Length >= _maxSlots || _clients.Any(c => Equals(c.Peer, e.RemotePeer.RemoteEndPoint)))
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;
            e.RemotePeer.Tag = GetUnusedPlayer();
            if (_game != null)
            {
                _clients.Add(new Client(e.RemotePeer.RemoteEndPoint, (Player)e.RemotePeer.Tag));
                _clients.Add(new Client(null, GetUnusedPlayer(), true));
                var players = _clients.Select(con => con.Player).ToList();
                foreach (var con in players)
                {
                    _game.AddPlayer(con);
                }
            }
        }

        private Player GetUnusedPlayer()
        {
            var p = _unusedPlayers.First(t => t.Enabled);
            _unusedPlayers.Remove(p);
            return p;
        }

        public void Send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            foreach (var client in _server.Connections)
            {
                _server.Send(new OutgoingMessage(bytes), client);
            }

        }

        public void Send(string message, RemotePeer peer)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            _server.Send(new OutgoingMessage(bytes), peer);
        }

        public void Send(byte status, RemotePeer peer)
        {
            var array = new byte[1];
            array[0] = status;
            _server.Send(new OutgoingMessage(array), peer);
        }

        private void MessageReceived(object sender, IncomingMessageEventArgs e)
        {
            var msg = Encoding.UTF8.GetString(e.Message.Data);
            Console.WriteLine(msg);
            var senderPeer = e.Message.RemotePeer;

            if (_commands.ContainsKey(msg))
            {
                var act = _commands[msg];
                act?.Invoke(senderPeer, msg);
            }
        }

        private void PlayerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            var client = _clients.FirstOrDefault(t => Equals(t.Peer, e.RemotePeer.RemoteEndPoint));
            var player = client.Player;
            _clients.Remove(client);
            _unusedPlayers.Add(player);
            _game.RemovePlayer(player);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _server.PeerJoined -= PlayerJoined;
                    _server.MessageArrived -= MessageReceived;
                    _server.PeerDisconnected -= PlayerDisconnected;
                    _server.Close();
                    _server.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public enum ServerMode
    {
        Internal,
        Global
    }
}

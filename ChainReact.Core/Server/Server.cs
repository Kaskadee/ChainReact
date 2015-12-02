using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        public ServerMode Mode { get; private set; }

        public Server(ServerMode mode, NetworkPeer.Protocol protocol, int port = 22794)
        {
            Mode = mode;
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

        public void Send(CommandProtocol protocol, byte[] message)
        {
            message = AppendProtocol(protocol, message);
            foreach (var peer in _server.Connections)
            {
                _server.Send(new OutgoingMessage(message), peer);
            }
        }

        public void Send(CommandProtocol protocol, byte[] message, RemotePeer peer)
        {
            message = AppendProtocol(protocol, message);
            _server.Send(new OutgoingMessage(message), peer);
        }

        public Client GetClient(RemotePeer peer)
        {
            var client = _clients.FirstOrDefault(t => Equals(t.Peer, peer.RemoteEndPoint));
            return client;
        }

        public List<Client> GetClients()
        {
            return _clients;
        }

        public void SetPlayer(Player p, RemotePeer peer)
        {
            var client = _clients.FirstOrDefault(t => Equals(t.Peer, peer.RemoteEndPoint));
            client.Player = p;
        }

        private byte[] AppendProtocol(CommandProtocol protocol, byte[] bytes)
        {
            var newArray = new byte[bytes.Length + 1];
            bytes.CopyTo(newArray, 1);
            newArray[0] = (byte)protocol;
            return newArray;
        }

        private void MessageReceived(object sender, IncomingMessageEventArgs e)
        {
            var senderPeer = e.Message.RemotePeer;
            var t = new Thread(delegate () { ServerCommands.HandleCommands(this, senderPeer, e.Message.Data); });
            t.Start();
        }

        private void PlayerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            var client = _clients.FirstOrDefault(t => Equals(t.Peer, e.RemotePeer.RemoteEndPoint));
            var player = client.Player;
            _clients.Remove(client);
            _unusedPlayers.Add(player);
            _game.RemovePlayer(player);
        }

        public void Restart()
        {
            foreach (var player in _game.Players)
            {
                player.ExecutedFirstPlace = false;
                player.Out = false;
                player.Save();
            }
            var newPlayers = GameSettings.Instance.Players;
            foreach (var player in newPlayers)
            {
                player.ExecutedFirstPlace = false;
                player.Out = false;
                player.Save();
            }
            _game = new ChainReactGame(newPlayers);
            SendInformations();
        }

        public void SendInformations()
        {
            var serializedMap = RemoteGame.GameMap.Serialize();
            var currentPlayer =
                Encoding.UTF8.GetBytes(RemoteGame.CurrentPlayer.Name + "|" +
                                       RemoteGame.CurrentPlayer.ColorName);
            var ready = new byte[1];
            ready[0] = 200;
            Send(CommandProtocol.GameOverData, Encoding.UTF8.GetBytes("false|null"));
            Send(CommandProtocol.MapData, serializedMap);
            Send(CommandProtocol.PlayerData, currentPlayer);
            Send(CommandProtocol.Ready, ready);
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

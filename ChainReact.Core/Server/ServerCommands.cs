using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainReact.Core.Game.Objects;
using Sharpex2D.Framework.Network;

namespace ChainReact.Core.Server
{
    public sealed class ServerCommands
    {
        private readonly Dictionary<string, Action<RemotePeer, string>> _dict;
        private readonly Server _server;

        public ServerCommands(Server serv, Dictionary<string, Action<RemotePeer, string>> dictToFill)
        {
            _server = serv;
            _dict = dictToFill;
        }

        public Dictionary<string, Action<RemotePeer, string>> RegisterCommands()
        {
            _dict.Add("request", InformationRequestReceived);
            _dict.Add("set", SetWabeReceived);
            _dict.Add("maprequest", MapRequestReceived);
            return _dict;
        }

        private void MapRequestReceived(RemotePeer sender, string msg)
        {
            var serialized = _server.RemoteGame.GameMap.Serialize();

            _server.Send("map:" + serialized);
        }

        private void InformationRequestReceived(RemotePeer sender, string msg)
        {
            var serialized = _server.RemoteGame.GameMap.Serialize();

            _server.Send("map:" + serialized, sender);
            _server.Send("currentplayer:" + _server.RemoteGame.CurrentPlayer.Name + ":" + _server.RemoteGame.CurrentPlayer.ColorName, sender);
            _server.Send(200, sender);
        }

        private void SetWabeReceived(RemotePeer sender, string msg)
        {
            var player = (Player)sender.Tag;
            if (_server.RemoteGame.CurrentPlayer.Id != player.Id)
            {
                var error = "It's not your turn!";
                _server.Send(error, sender);
                return;
            }
            var splitted = msg.Split(':');
            var stringX = splitted[0];
            var stringY = splitted[1];
            var stringFieldId = splitted[2];
            int x;
            int y;
            int fieldId;
            if (int.TryParse(stringX, out x) && int.TryParse(stringY, out y) && int.TryParse(stringFieldId, out fieldId))
            {
                var wabe = _server.RemoteGame.GameMap[x, y];
                var field = _server.RemoteGame.GameMap.GetField(wabe, fieldId);
                string error;
                _server.RemoteGame.Set(player.Id, wabe, field, out error);
                var mapSerialized = _server.RemoteGame.GameMap.Serialize();
                _server.Send("map:" + mapSerialized);
                if (!string.IsNullOrEmpty(error))
                {
                    _server.Send("error:" + error, sender);
                }
                _server.Send("currentplayer:" + _server.RemoteGame.CurrentPlayer.Name + ":" + _server.RemoteGame.CurrentPlayer.ColorName);
                if (_server.RemoteGame.GameOver)
                {
                    _server.Send("gameover");
                    _server.Send("winner:" + _server.RemoteGame.Winner.Name);
                }
            }
        }
    }
}

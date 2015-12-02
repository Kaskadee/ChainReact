using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ChainReact.Core.Game.Objects;
using Sharpex2D.Framework.Network;

namespace ChainReact.Core.Server
{
    public static class ServerCommands
    {
        public static void HandleCommands(Server server, RemotePeer sender, byte[] bytes)
        {
            var protocol = GetProtocol(ref bytes);
            var client = server.GetClient(sender);
            var player = client.Player;
            switch (protocol)
            {
                case CommandProtocol.MapRequest:
                    var reason = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine("Sending map data to " + player.Name + " as requested by client: " + reason);
                    var serialized = server.RemoteGame.GameMap.Serialize();
                    server.Send(CommandProtocol.MapData, serialized);
                    break;
                case CommandProtocol.Request:
                    server.SendInformations();
                    break;
                case CommandProtocol.SetData:
                    if (server.RemoteGame.CurrentPlayer.Id != player.Id)
                    {
                        var error = Encoding.UTF8.GetBytes("It's not your turn!");
                        server.Send(CommandProtocol.Error, error, sender);
                        return;
                    }
                    var msg = Encoding.UTF8.GetString(bytes);
                    var splitted = msg.Split('|');
                    var stringX = splitted[0];
                    var stringY = splitted[1];
                    var stringFieldId = splitted[2];
                    int x;
                    int y;
                    int fieldId;
                    if (int.TryParse(stringX, out x) && int.TryParse(stringY, out y) && int.TryParse(stringFieldId, out fieldId))
                    {
                        var wabe = server.RemoteGame.GameMap[x, y];
                        var field = server.RemoteGame.GameMap.GetField(wabe, fieldId);
                        string error;
                        server.RemoteGame.Set(player.Id, wabe, field, out error);

                        var mapSerialized = server.RemoteGame.GameMap.Serialize();
                        server.Send(CommandProtocol.MapData, mapSerialized);


                        if (!string.IsNullOrEmpty(error))
                        {
                            server.Send(CommandProtocol.Error, Encoding.UTF8.GetBytes(error), sender);
                        }
                        var currentPlayerData =
                            Encoding.UTF8.GetBytes(server.RemoteGame.CurrentPlayer.Name + "|" +
                                                   server.RemoteGame.CurrentPlayer.ColorName);
                        if (server.Mode == ServerMode.Internal)
                        {
                            client.Player = server.RemoteGame.CurrentPlayer;
                        }
                        server.Send(CommandProtocol.PlayerData, currentPlayerData);
                        if (server.RemoteGame.GameOver)
                        {
                            var gameOver = "true|" + server.RemoteGame.Winner.Name;
                            server.Send(CommandProtocol.GameOverData, Encoding.UTF8.GetBytes(gameOver));
                        }
                    }
                    break;
                case CommandProtocol.Restarting:
                    if (server.RemoteGame.GameOver)
                    {
                        server.Restart();
                    }
                    break;
                default:
                    return;
            }
        }

        public static CommandProtocol GetProtocol(ref byte[] bytes)
        {
            var protocol = (CommandProtocol)bytes[0];
            var newArray = new byte[bytes.Length - 1];
            Buffer.BlockCopy(bytes, 1, newArray, 0, newArray.Length);
            bytes = newArray;
            return protocol;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;

namespace ChainReact.Utilities
{
    public sealed class DefaultCommands
    {
        private readonly MainGame _game;

        public DefaultCommands(MainGame game)
        {
            _game = game;
        }

        public void RegisterCommands()
        {
            _game.RegisterCommand("error", ErrorReceived);
            _game.RegisterCommand("map", MapReceived);
            _game.RegisterCommand("gameover", GameOverReceived);
            _game.RegisterCommand("currentplayer", CurrentPlayerReceived);
            _game.RegisterCommand("winner", WinnerReceived);
        }

        private void CurrentPlayerReceived(string msg)
        {
            var splitted = msg.Split(':');
            _game.SetCurrentPlayer(splitted[1], splitted[2]);
        }

        private void WinnerReceived(string msg)
        {
            var splitted = msg.Split(':');
            _game.SetWinner(splitted[1]);
        }

        private void MapReceived(string msg)
        {
            var splitted = msg.Split(':');
            var mapBase = splitted[1];
            var formatter = new BinaryFormatter();
            try
            {
                var map = (Map) formatter.DeepDeserialize(mapBase);
                _game.SetMap(map);
            }
            catch (FormatException)
            {
                // Invalid map string received. Request map again
                _game.Send("maprequest");
                Console.WriteLine(@"Invalid map received!");
            }
           
        }

        private void GameOverReceived(string msg)
        {
            var splitted = msg.Split(':');
            var gameOverString = splitted[1];
            bool gameOver;
            if (bool.TryParse(gameOverString, out gameOver))
            {
                _game.SetGameover(gameOver);
            }
        }

        private void ErrorReceived(string msg)
        {
            var splitted = msg.Split(':');
            var error = splitted[1];
            _game.SetMessage(error);
        }
    }
}

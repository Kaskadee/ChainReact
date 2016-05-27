using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework;

namespace ChainReact.Core.Game
{
    public class ChainReactGame
    {
        public const float WabeSize = 96.0F;

        public GameQueue Queue { get; private set; }

        public Player CurrentPlayer { get; set; }
        public List<Player> Players { get; private set; }
        private Dictionary<Player, bool> _executedFirstPlace;
        private Dictionary<Player, bool> _isOut;  

        public bool GameOver { get; private set; }
        public string Message { get; private set; }
        public Player Winner { get; private set; }

        public bool Playing { get; private set; }

        public Map GameMap { get; set; }

        public ChainReactGame(bool skipAnimation, bool output)
        {
            GameMap = new Map(this, skipAnimation, output);
        }

        public void Initialize(IEnumerable<Player> players)
        {
            Queue = new GameQueue();
            Players = players.ToList();
            _executedFirstPlace = new Dictionary<Player, bool>();
            _isOut = new Dictionary<Player, bool>();
            foreach (var p in Players)
            {
                _executedFirstPlace.Add(p, false);
                _isOut.Add(p, false);
            }
            CurrentPlayer = Players.First();
            
            Playing = true;
        }

        public void RemovePlayer(Player p)
        {
            Players.Remove(p);

            if (Players.Count < 2)
            {
                Message = "All opponents have left the game!";
                GameOver = true;
                Winner = Players.FirstOrDefault();
                return;
            }

            if (CurrentPlayer.Id == p.Id)
            {
                CurrentPlayer = Players.NextOfPlayer(p, _isOut);
            }
        }

        public void StopGame()
        {
            Playing = false;
        }

        public bool Set(string playerId, int x, int y, out string error)
        {
            if (x > GameMap.Wabes.GetLength(0) || x < 0)
                throw new IndexOutOfRangeException("x");
            if (y > GameMap.Wabes.GetLength(1) || y < 0)
                throw new IndexOutOfRangeException("y");
            var wabe = GameMap.Wabes[x, y];
            if (CurrentPlayer.Id != playerId)
            {
                error = "You aren't the current player";
                return false;
            }
            if (wabe.Owner != null && wabe.Owner.Id != playerId)
            {
                error = "This wabe is already owned by another player";
                return false;
            }
            if (!_executedFirstPlace[CurrentPlayer]) _executedFirstPlace[CurrentPlayer] = true;
            wabe.Set(CurrentPlayer);
            error = null;
            CurrentPlayer = Players.NextOfPlayer(CurrentPlayer, _isOut);
            return true;
        }

        public bool Set(string playerId, Wabe wabe, WabeField field, out string error)
        {
            if (wabe.X > GameMap.Wabes.GetLength(0) || wabe.X < 0)
                throw new IndexOutOfRangeException("x");
            if (wabe.Y > GameMap.Wabes.GetLength(1) || wabe.Y < 0)
                throw new IndexOutOfRangeException("y");
            if (CurrentPlayer.Id != playerId)
            {
                error = "You aren't the current player";
                return false;
            }
            if (wabe.Owner != null && wabe.Owner.Id != playerId)
            {
                error = "This wabe is already owned by another player";
                return false;
            }
            if (!_executedFirstPlace[CurrentPlayer]) _executedFirstPlace[CurrentPlayer] = true;
            wabe.Set(CurrentPlayer, field);
            error = null;
            CurrentPlayer = Players.NextOfPlayer(CurrentPlayer, _isOut);
            return true;
        }

        public void SetGameMessage(string message)
        {
            Message = message;
        }

        public bool CheckWin()
        {
            if (GameOver) return true;
            var wabeList = GameMap.Wabes.Cast<Wabe>().ToList();
            foreach (var player in Players)
            {
                if (!_executedFirstPlace[player] || _isOut[player]) continue;
                if(wabeList.Count(w => w.Owner?.Id == player.Id) == 0)
                {
                    var reason = $"{player.Name} don't have any more wabes and has been eliminated!";
                    Message = reason;
                    if (CurrentPlayer.Id == player.Id)
                    {
                        CurrentPlayer = Players.NextOfPlayer(CurrentPlayer, _isOut);
                    }
                    _isOut[player] = true;
                }
            }

            foreach (var player in Players)
            {
                if (Players.All(p => _executedFirstPlace[p]) && Players.Count(p => !_isOut[p]) == 1 && !_isOut[player])
                {
                    var reason = $"{player.Name} is last man standing!";
                    GameOver = true;
                    Message = reason;
                    Winner = player;
                    Winner.Wins++;
                    return true;
                }
                if (wabeList.Count(w => w.Owner != null && w.Owner == player) >= 25)
                {
                    var reason = $"{player.Name} has captured 25 wabes.";
                    GameOver = true;
                    Message = reason;
                    Winner = player;
                    Winner.Wins++;
                    return true;
                }
            }
            return false;
        }
    }
}

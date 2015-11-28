using System;
using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;

namespace ChainReact.Core.Game
{
    public class ChainReactGame
    {
        public const float WabeSize = 64.0F;
        public const float ScalingFactor = 1.5F;
        public const float FullSize = WabeSize*ScalingFactor;

        public GameQueue Queue { get; }

        public Player CurrentPlayer { get; private set; }
        public List<Player> Players { get; }

        public bool GameOver { get; private set; }
        public string Message { get; private set; }
        public Player Winner { get; private set; }

        public Map GameMap { get; private set; }
        public GameState State { get; private set; }

        public ChainReactGame()
        {
            Players = new List<Player>();
            State = GameState.WaitingForPlayers;
        }

        public ChainReactGame(IEnumerable<Player> players)
        {
            Queue = new GameQueue();
            Players = players.OrderBy(p => p.Id).ToList();
            State = players.Count() >= 2 ? GameState.Starting : GameState.WaitingForPlayers;
            if (State == GameState.Starting)
            {
                CurrentPlayer = Players.First(t => !t.Out);
                GameMap = new Map(this, "ExplosionSound");
            }
        }

        public void AddPlayer(Player p)
        {
            Players.Add(p);
            State = Players.Count() >= 2 ? GameState.Starting : GameState.WaitingForPlayers;
            if (State == GameState.Starting)
            {
                CurrentPlayer = Players.First(t => !t.Out);
                GameMap = new Map(this, "ExplosionSound");
            }
        }

        public void RemovePlayer(Player p)
        {
            Players.Remove(p);
            State = Players.Count() >= 2 ? GameState.Starting : GameState.WaitingForPlayers;
            if (State == GameState.WaitingForPlayers)
            {
                CurrentPlayer = null;
                GameMap = null;
            }
        }

        public bool Set(string player, int x, int y, out string error)
        {
            var p = Players.Find(item => item.Name == player);
            if (p == null)
                throw new InvalidOperationException($"Couldn't find player {player}");
            var set = Set(p.Id, x, y, out error);
            return set;
        }

        public bool Set(int player, int x, int y, out string error)
        {
            if (x > GameMap.GetLengthX() || x < 0)
                throw new IndexOutOfRangeException("x");
            if (y > GameMap.GetLengthY() || y < 0)
                throw new IndexOutOfRangeException("y");
            var wabe = GameMap[x, y];
            if (CurrentPlayer.Id != player)
            {
                error = "You aren't the current player";
                return false;
            }
            if (wabe.Owner != null && wabe.Owner.Id != player)
            {
                error = "This wabe is already owned by another player";
                return false;
            }
            if (!CurrentPlayer.ExecutedFirstPlace) CurrentPlayer.ExecutedFirstPlace = true;
            wabe.Set(CurrentPlayer);
            error = null;
            CurrentPlayer = Players.NextOfPlayer(CurrentPlayer);
            return true;
        }

        public bool Set(int player, Wabe wabe, WabeField field, out string error)
        {
            if (wabe.X > GameMap.GetLengthX() || wabe.X < 0)
                throw new IndexOutOfRangeException("x");
            if (wabe.Y > GameMap.GetLengthY() || wabe.Y < 0)
                throw new IndexOutOfRangeException("y");
            if (CurrentPlayer.Id != player)
            {
                error = "You aren't the current player";
                return false;
            }
            if (wabe.Owner != null && wabe.Owner.Id != player)
            {
                error = "This wabe is already owned by another player";
                return false;
            }
            if (!CurrentPlayer.ExecutedFirstPlace) CurrentPlayer.ExecutedFirstPlace = true;
            wabe.Set(CurrentPlayer, field);
            error = null;
            CurrentPlayer = Players.NextOfPlayer(CurrentPlayer);
            return true;
        }

        public bool CheckWin()
        {
            if (GameOver) return true;
            var wabeList = GameMap.ToList();
            foreach (var player in Players)
            {
                if (!player.ExecutedFirstPlace || player.Out || wabeList.Count(w => w.Owner == player) != 0) continue;
                var reason = $"{player.Name} don't have any more wabes and has been eliminated!";
                Message = reason;
                if (CurrentPlayer.Id == player.Id)
                {
                    CurrentPlayer = Players.NextOfPlayer(CurrentPlayer);
                }
                player.Out = true;
            }
           
            foreach (var player in Players)
            {
                if (Players.All(p => p.ExecutedFirstPlace) && Players.Count(p => !p.Out) == 1 && !player.Out)
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

    public enum GameState
    {
        WaitingForPlayers,
        Starting,
        Paused,
        Stopping,
        Stopped,
        Running
    }
}

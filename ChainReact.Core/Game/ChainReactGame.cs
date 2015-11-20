using System;
using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Game.Animations;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game
{
    public class ChainReactGame
    {
        public Sharpex2D.Framework.Game Game { get; private set; }
        public GameQueue Queue { get; }

        public Player CurrentPlayer { get; private set; }
        public List<Player> Players { get; }
        public Wabe[,] Wabes { get; }

        public bool GameOver { get; private set; }
        public string Message { get; private set; }

        public event EventHandler<DrawRequestedEventArgs> RequestDraw;

        public ChainReactGame(Sharpex2D.Framework.Game game, IEnumerable<Player> players, Vector2 size)
        {
            Game = game;
            Queue = new GameQueue();
            Players = players.OrderBy(p => p.Id).ToList();
            Wabes = new Wabe[6, 6];
            for (var x = 0; x <= 5; x++)
            {
                for (var y = 0; y <= 5; y++)
                {
                    WabeType type;
                    if (x == 0 && y == 0 || x == Wabes.GetLength(0) - 1 && y == 0 || x == 0 && y == Wabes.GetLength(1) - 1 || x == Wabes.GetLength(0) - 1 && y == Wabes.GetLength(1) - 1)
                    {
                        type = WabeType.TwoWabe;
                    }
                    else if (x == 0 || y == 0 || x == Wabes.GetLength(0) - 1 || y == Wabes.GetLength(1) - 1)
                    {
                        type = WabeType.ThreeWabe;
                    }
                    else
                    {
                        type = WabeType.FourWabe;
                    }
                    Wabes[x, y] = new Wabe(this, type, x, y, size, "ExplosionSound");
                }
            }
            CurrentPlayer = Players.First(t => !t.Out);
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
            if (x > Wabes.GetLength(0) || x < 0)
                throw new IndexOutOfRangeException("x");
            if (y > Wabes.GetLength(1) || y < 0)
                throw new IndexOutOfRangeException("y");
            var wabe = Wabes[x, y];
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
            if (wabe.X > Wabes.GetLength(0) || wabe.X < 0)
                throw new IndexOutOfRangeException("x");
            if (wabe.Y > Wabes.GetLength(1) || wabe.Y < 0)
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

        public bool CheckWin(out Player winner)
        {
            winner = null;
            if (GameOver) return true;
            var wabeList = Wabes.Cast<Wabe>().ToList();
            foreach (var player in Players)
            {
                if (!player.ExecutedFirstPlace || player.Out) continue;
                if (Wabes.Cast<Wabe>().Count(w => w.Owner == player) == 0)
                {
                    var reason = $"{player.Name} don't have any more wabes and has been eliminated!";
                    Message = reason;
                    if (CurrentPlayer.Id == player.Id)
                    {
                        CurrentPlayer = Players.NextOfPlayer(CurrentPlayer);
                    }
                    player.Out = true;
                }
            }
           
            foreach (var player in Players)
            {
                if (Players.All(p => p.ExecutedFirstPlace) && Players.Count(p => !p.Out) == 1 && !player.Out)
                {
                    var reason = $"{player.Name} is last man standing!";
                    GameOver = true;
                    Message = reason;
                    winner = player;
                    winner.Wins++;
                    return true;
                }
                if (wabeList.Count(w => w.Owner != null && w.Owner == player) >= 25)
                {
                    var reason = $"{player.Name} has captured 25 wabes.";
                    GameOver = true;
                    Message = reason;
                    winner = player;
                    winner.Wins++;
                    return true;
                }
            }
            return false;
        }

        public List<Wabe> FindNearWabes(int x, int y)
        {
            var leftX = x - 1;
            var rightX = x + 1;
            var upY = y - 1;
            var downY = y + 1;
            var result = new List<Wabe>();
            if (leftX >= 0)
            {
                result.Add(Wabes[leftX, y]);
            }
            if (rightX <= Wabes.GetLength(0) - 1)
            {
                result.Add(Wabes[rightX, y]);
            }
            if (upY >= 0)
            {
                result.Add(Wabes[x, upY]);
            }
            if (downY <= Wabes.GetLength(1) - 1)
            {
                result.Add(Wabes[x, downY]);
            }
            return result;
        }

        public Wabe ConvertAbsolutePositionToWabe(Vector2 vector, float wabesize)
        {
            if (Math.Abs(vector.X) < 1 || Math.Abs(vector.Y) < 1) return null;
            var x = (vector.X / wabesize) - 1;
            var y = (vector.Y / wabesize) - 1;
            if (x < 0 || y < 0) return null;
            var relativeX = (int) Math.Floor(x);
            var relativeY = (int) Math.Floor(y);
            if (relativeX > (Wabes.GetLength(0) - 1) || relativeY > (Wabes.GetLength(1) - 1)) return null;
            return Wabes[relativeX, relativeY];
        }

        public void InvokeDrawRequest(object sender, DrawRequestedEventArgs args)
        {
            RequestDraw?.Invoke(this, args);
        }
    }
}

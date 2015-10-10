using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void SimulateGame()
        {
            var players = new List<Player> { new Player(1, "Dummy1", Color.Green), new Player(2, "Dummy2", Color.Red), new Player(3, "Dummy3", Color.Yellow), new Player(4, "Dummy4", Color.Blue) };
            var game = new ChainReactGame(players);
            Debug.Assert(game.CurrentPlayer == players.First());
            Debug.Assert(game.Wabes.Length == 36);
            var twoWabeCount = 0;
            var threeWabeCount = 0;
            var fourWabeCount = 0;
            foreach (var wabe in game.Wabes)
            {
                switch (wabe.Type)
                {
                    case WabeType.FourWabe:
                        fourWabeCount++;
                        break;
                    case WabeType.ThreeWabe:
                        threeWabeCount++;
                        break;
                    case WabeType.TwoWabe:
                        twoWabeCount++;
                        break;
                }
            }
            Debug.Assert(twoWabeCount == 4);
            Debug.Assert(threeWabeCount == 16);
            Debug.Assert(fourWabeCount == 16);
            var nearWabes1 = game.FindNearWabes(3, 3);
            Debug.Assert(nearWabes1.Count == 4);
            var nearWabes2 = game.FindNearWabes(0, 0);
            Debug.Assert(nearWabes2.Count == 2);
            foreach (var wabe in nearWabes1)
            {
                Debug.WriteLine(wabe.X + " - " + wabe.Y);
            }
            foreach (var wabe in nearWabes2)
            {
                Debug.WriteLine(wabe.X + " - " + wabe.Y);
            }
            string error;
            game.Set(1, 3, 3, out error);
            Debug.Assert(error == null);
            Debug.Assert(game.CurrentPlayer.Id == 2);
            game.Set(2, 0, 0, out error);
            Debug.Assert(error == null);
            Debug.Assert(game.CurrentPlayer.Id == 3);
            game.Set(2, 4, 4, out error);
            Debug.Assert(error != null);
            Debug.Assert(game.CurrentPlayer.Id == 3);
            game.Set(3, 3, 3, out error);
            Debug.Assert(error != null);
            Debug.Assert(game.CurrentPlayer.Id == 3);
            game.Set(3, 5, 3, out error);
            Debug.Assert(game.CurrentPlayer.Id == 4);
            game.Set(4, 5, 0, out error);
            game.Set(1, 3, 3, out error);
            Debug.Assert(error == null);
            Debug.Assert(game.CurrentPlayer.Id == 2);
            Debug.Assert(game.Wabes[3,3].PoweredSpheres == 2);

            // Explode test
            game.Set(2, 0, 0, out error);
            var wabeEx = game.Wabes[0, 0];
            var wabeNext = game.Wabes[0, 1];
            Debug.Assert(error == null);
            Debug.Assert(game.CurrentPlayer.Id == 3);
            Debug.Assert(wabeEx.Owner == null);
            Debug.Assert(wabeEx.PoweredSpheres == 0);
            Debug.Assert(wabeNext.Owner.Id == 2);
            Debug.Assert(wabeNext.PoweredSpheres == 1);
            

            // Explode test 2
            var capturedWabe = game.Wabes[4, 0];
            game.Set(3, 4, 0, out error);
            game.Set(4, 5, 0, out error);
            Debug.Assert(capturedWabe.Owner.Id == 4);
            Debug.Assert(capturedWabe.PoweredSpheres == 2);

        }

        [TestMethod]
        public void CombineDirectionEnum()
        {
            var direction = WabeDirection.Up | WabeDirection.Left;
            var direction2 = WabeDirection.Up & WabeDirection.Down;
            Debug.Assert(direction.HasFlag(WabeDirection.Up) && direction.HasFlag(WabeDirection.Left));
            Debug.Assert(direction2.HasFlag(WabeDirection.Up) && direction2.HasFlag(WabeDirection.Left));
        }

        [TestMethod]
        public void OperatorTest()
        {
            for (var i = 0; i < 25; i++)
                Console.WriteLine(((i + 1) & 1));
        }

        [TestMethod]
        public void Vector2EqualsCheck()
        {
            var a = new Vector2(0, 0);
            var b = Vector2.Zero;
            Debug.Assert(a.Equals(b));
            Debug.Assert(a == b, "a == b");
            Debug.Assert(b == a, "b == a");
            //Debug.Assert(a != b); // false
        }
    }
}

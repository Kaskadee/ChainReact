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

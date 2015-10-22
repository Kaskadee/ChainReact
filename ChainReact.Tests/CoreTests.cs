using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ChainReact.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void EnumerableReference()
        {
            var foo1 = new Foo { Bar = "1"};
            var foo2 = new Foo { Bar = "2" };
            var foo3 = new Foo {Bar = "3"};
            var fooList = new List<Foo> { foo1, foo2, foo3 };
            var fooMachine1 = new FooMachine();
            var fooMachine2 = new FooMachine();
            fooMachine1.UseList(fooList, false);
            fooMachine2.UseList(fooList, true);
            fooMachine1.UseList(fooList, false);
        }
    }

    public class FooMachine
    {
        public void UseList(List<Foo> fooList, bool change)
        {
            var random = new Random();
            if (change)
            {
                foreach (var foo in fooList)
                {
                    foo.SetBar(random.Next(0, 1000000).ToString());
                }
            }
            foreach (var foo in fooList)
            {
                Debug.WriteLine(foo.Bar);
            }
        }
    }

    public struct Foo
    {
        public string Bar { get; set; }

        public void SetBar(string bar)
        {
            Bar = bar;
        }
    }
}

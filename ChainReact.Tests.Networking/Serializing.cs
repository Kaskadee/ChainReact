using System;
using System.Diagnostics;
using ChainReact.Core;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Tests.Networking
{
    [TestClass]
    public class Serializing
    {
        [TestMethod]
        public void MapSerialize()
        {
            try
            {
                var chainReactGame = new ChainReactGame(true, false);
                var serialized = chainReactGame.GameMap.Serialize();
                var deserialized = Map.Deserialize(serialized);
                Console.WriteLine(deserialized.GetHashString());
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                Debug.Fail(ex.Message);
            }
           
        }
    }
}

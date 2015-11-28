﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework;

namespace ChainReact.Core.Game.Objects
{
    [Serializable]
    public sealed class Map
    {
        public const int DefaultLengthX = 6;
        public const int DefaultLengthY = 6;

        public Wabe[,] Wabes { get; set; }

        public Wabe this[int x, int y] => Wabes[x, y];

        public Map(ChainReactGame game, string soundResource)
        {
            Wabes = new Wabe[6, 6];
            for (var x = 0; x <= 5; x++)
            {
                for (var y = 0; y <= 5; y++)
                {
                    WabeType type;
                    if (x == 0 && y == 0 || x == Wabes.GetLength(0) - 1 && y == 0 ||
                        x == 0 && y == Wabes.GetLength(1) - 1 ||
                        x == Wabes.GetLength(0) - 1 && y == Wabes.GetLength(1) - 1)
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
                    Wabes[x, y] = new Wabe(game, type, x, y, soundResource);
                }
            }
        }

        public List<Wabe> GetNearWabes(int x, int y)
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

        public Wabe AbsoluteToWabe(Vector2 vector)
        {
            var wabesize = ChainReactGame.FullSize;
            if (Math.Abs(vector.X) < 1 || Math.Abs(vector.Y) < 1) return null;
            var x = (vector.X / wabesize) - 1;
            var y = (vector.Y / wabesize) - 1;
            if (x < 0 || y < 0) return null;
            var relativeX = (int)Math.Floor(x);
            var relativeY = (int)Math.Floor(y);
            if (relativeX > (GetLengthX() - 1) || relativeY > (GetLengthY() - 1)) return null;
            return this[relativeX, relativeY];
        }

        public WabeField GetField(Wabe wabe, int id)
        {
            return this[wabe.X, wabe.Y].Fields[id];
        }

        public int GetLengthX()
        {
            return Wabes.GetLength(0);
        }

        public int GetLengthY()
        {
            return Wabes.GetLength(1);
        }

        public List<Wabe> ToList()
        {
            return Wabes.Cast<Wabe>().ToList();
        } 

        public string Serialize()
        {
            var formatter = new BinaryFormatter();
            return formatter.DeepSerialize(this);
        }

        public static Map Deserialize(string serialized)
        {
            var formatter = new BinaryFormatter();
            return (Map) formatter.DeepDeserialize(serialized);
        }
    }
}

using System;
using ChainReact.Core.Game.Field;
using Sharpex2D.Framework;

namespace ChainReact.Core.Game.Layout
{
    [Serializable]
    public class WabeLayout
    {
        [NonSerialized]
        private readonly Wabe _wabe;

        public WabeFieldType[] Fields { get; private set; }
        public WabeDirection Direction { get; }

        public WabeLayout(Wabe wabe, WabeDirection direction)
        {
            if (wabe == null)
                throw new ArgumentNullException(nameof(wabe));
            _wabe = wabe;
            Direction = direction;
            GenerateFieldArray();
        }

        public WabeLayout(Wabe wabe, Vector2 position)
        {
            if (wabe == null)
                throw new ArgumentNullException(nameof(wabe));
            _wabe = wabe;
            Direction = CalculateDirection(position);
            GenerateFieldArray();
        }

        public void GenerateFieldArray()
        {
            Fields = new WabeFieldType[9];

            for (var i = 0; i <= 8; i++)
            {
                Fields[i] = CalculateFieldType(_wabe.Type, Direction, i);
            }
        }

        private WabeDirection CalculateDirection(Vector2 position)
        {
            var min = (int)position.X;
            var max = (int)position.Y;
            switch (_wabe.Type)
            {
                case WabeType.TwoWabe:
                    if (_wabe.X == min)
                    {
                        if (_wabe.Y == min) return WabeDirection.Left | WabeDirection.Up;
                        if (_wabe.Y == max) return WabeDirection.Left | WabeDirection.Down;
                    }
                    else if (_wabe.X == max)
                    {
                        if (_wabe.Y == min) return WabeDirection.Right | WabeDirection.Up;
                        if (_wabe.Y == max) return WabeDirection.Right | WabeDirection.Down;
                    }
                    break;
                case WabeType.ThreeWabe:
                    if (_wabe.X == min) return WabeDirection.Left;
                    if (_wabe.X == max) return WabeDirection.Right;
                    if (_wabe.Y == min) return WabeDirection.Up;
                    if (_wabe.Y == max) return WabeDirection.Down;
                    break;
                case WabeType.FourWabe:
                    break;
            }
            return WabeDirection.Mid;
        }

        private WabeFieldType CalculateFieldType(WabeType type, WabeDirection direction, int index)
        {
            if (index == 4) return WabeFieldType.Center;
            if (0 != ((index + 1) & 1)) return WabeFieldType.Unused;
            switch (type)
            {
                case WabeType.TwoWabe:
                    switch (direction)
                    {
                        case WabeDirection.Up | WabeDirection.Left:
                            return (0 == (index & 4)) ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                        case WabeDirection.Down | WabeDirection.Right:
                            return (0 != (index & 4)) ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                        case WabeDirection.Down | WabeDirection.Left:
                            return (0 != (index & 2)) ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                        case WabeDirection.Up | WabeDirection.Right:
                            return (0 == (index & 2)) ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                    }
                    break;
                case WabeType.ThreeWabe:
                    switch (direction)
                    {
                        case WabeDirection.Left:
                            return index == 3 ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                        case WabeDirection.Right:
                            return index == 5 ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                        case WabeDirection.Up:
                            return index == 1 ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                        case WabeDirection.Down:
                            return index == 7 ? WabeFieldType.Unused : WabeFieldType.Unpowered;
                    }
                    break;
                case WabeType.FourWabe:
                    if (direction == WabeDirection.Mid) return WabeFieldType.Unpowered;
                    break;
            }
            throw new ArgumentException("The given index or direction does not match any field type.", nameof(index));
        }
    }
}

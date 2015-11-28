using System;

namespace ChainReact.Core.Game.Field
{
    [Serializable]
    public class WabeField
    {
        public int Id { get; set; }
        public WabeFieldType Type { get; set; }

        public WabeField(WabeFieldType type, int id)
        {
            Id = id;
            Type = type;
        }
    }

    [Serializable]
    public enum WabeFieldType
    {
        Unused,
        Unpowered,
        Powered,
        Center
    }

    [Flags]
    [Serializable]
    public enum WabeDirection
    {
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Mid = 16
    }
}

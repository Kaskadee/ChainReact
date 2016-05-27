using System;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Serialization;
using ChainReact.Core.Utilities;
using Newtonsoft.Json;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Objects
{
    public class Player
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("player_name")]
        public string Name { get; set; }
        [JsonProperty("color")]
        public Color Color { get; set; }
        [JsonProperty("color_name")]
        public string ColorName { get; set; }
        [JsonProperty("win_count")]
        public ushort Wins { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        public Player()
        {
            Id = "0";
            Name = "0";
            Color = Color.Transparent;
            ColorName = "Transparent";
            Wins = 0;
            Enabled = false;
        }

        public Player(string id, string name, ColorInformation information)
        {
            if(id == string.Empty || id == Guid.Empty.ToString())
                throw new ArgumentNullException(nameof(id));
            Color = information.Color;
            ColorName = information.ColorName;
            Id = id;
            Name = name;
        }

        public Player(string id, string name, ColorInformation information, ushort wins)
        {
            if (id == string.Empty || id == Guid.Empty.ToString())
                throw new ArgumentNullException(nameof(id));
            Color = information.Color;
            ColorName = information.ColorName;
            Id = id;
            Name = name;
            Wins = wins;
        }

        public string GetColorString()
        {
            return !string.IsNullOrEmpty(ColorName) ? ColorName : "Unknown";
        }
    }
}

using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using ChainReact.Core.Rendering;

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
			Color = Color.Transparent;
			ColorName = "Transparent";
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


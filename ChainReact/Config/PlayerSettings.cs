using System;
using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Game.Objects;
using Microsoft.Xna.Framework;
using ChainReact.Core.Rendering;
using Newtonsoft.Json;

namespace ChainReact
{
	public class PlayerSettings
	{
		[JsonIgnore]
		public List<Player> EnabledPlayers => AvailablePlayers.Where(p => p.Enabled).ToList();
		[JsonIgnore]
		public List<Player> DefaultPlayers => new List<Player> {
			new Player("p1", "Player1", AvailableColor[0]) { Enabled = true },
			new Player("p2", "Player2", AvailableColor[1]) { Enabled = true },
			new Player("p3", "Player3", AvailableColor[2]),
			new Player("p4", "Player4", AvailableColor[3])
		};
		[JsonIgnore]
		public List<ColorInformation> AvailableColor => new List<ColorInformation> {
			new ColorInformation(Color.Green, "Green"),
			new ColorInformation(Color.Red, "Red"),
			new ColorInformation(Color.Blue, "Blue"),
			new ColorInformation(Color.Orange, "Orange")
		};

		public List<Player> AvailablePlayers { get; set; } = new List<Player>();

		public PlayerSettings ()
		{
			AvailablePlayers.AddRange (DefaultPlayers);

		}
	}
}


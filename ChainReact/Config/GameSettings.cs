using System;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Linq;

namespace ChainReact
{
	public class GameSettings
	{
		#region Constants
		public const int ProtocolVersion = 2;
		public const int DefaultServerPort = 38589;

		public const int SignalNumber = 3010;
		#endregion

		private static GameSettings _instance;
		[JsonIgnore]
		public static GameSettings Instance => _instance ?? (_instance = new GameSettings());

		public PlayerSettings Players { get; set; }

		public bool FieldLines { get; set; } = true;
		public bool WabeLines { get; set; } = true;
		public bool BorderLines { get; set; } = true;

		public int MaximumPlayers { get; set; }

		[JsonIgnore]
		public IPAddress Address { get; set; } = IPAddress.Loopback;

		private GameSettings ()
		{
			Players = new PlayerSettings ();
		}


		public void Save(FileInfo info)
		{
			if (info.DirectoryName != null && !Directory.Exists(info.DirectoryName))
			{
				Directory.CreateDirectory(info.DirectoryName);
			}

			var json = JsonConvert.SerializeObject(this, Formatting.Indented);
			using (var fs = new FileStream(info.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
			{
				using (var sw = new StreamWriter(fs))
				{
					sw.Write(json);
				}
			}
		}

		public void Load(FileInfo info, DirectoryInfo players)
		{
			if (info.DirectoryName != null && !Directory.Exists(info.DirectoryName) || !File.Exists(info.FullName))
			{
				LoadDefaults(info);
				return;
			}
			using (var fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var sr = new StreamReader(fs))
				{
					var json = sr.ReadToEnd();
					var jsonClass = JsonConvert.DeserializeObject<GameSettings>(json);
					ApplyValues(jsonClass);
				}
			}

			if (Players.AvailablePlayers.Count <= 0)
			{
				Players.AvailablePlayers.AddRange(Players.DefaultPlayers);
			}
		}

		public void LoadDefaults(FileInfo info)
		{
			if (info.DirectoryName != null) Directory.CreateDirectory(info.DirectoryName);
			FieldLines = true;
			BorderLines = true;
			WabeLines = true;
			MaximumPlayers = 2;
			if (Players.AvailablePlayers.Count <= 0)
			{
				Players.AvailablePlayers.AddRange(Players.DefaultPlayers);
			}
			Save(info);
		}

		private void ApplyValues(GameSettings settings)
		{
			Players.AvailablePlayers = settings.Players.AvailablePlayers;
			FieldLines = settings.FieldLines;
			WabeLines = settings.WabeLines;
			BorderLines = settings.BorderLines;
			MaximumPlayers = settings.MaximumPlayers;
			if (MaximumPlayers < 2 || MaximumPlayers > 4)
			{
				MaximumPlayers = 2;
			}
		} 
	}
}


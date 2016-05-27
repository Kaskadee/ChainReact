using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using Newtonsoft.Json;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core
{
    public class GameSettings
    {
        #region Constants
        public const int ProtocolVersion = 1;
        public const int DefaultPortServer = 38589;

        public const short SignalNumber = 21745;
        #endregion

        private static GameSettings _settings;

        #region Ignored Values
        [JsonIgnore]
        public static GameSettings Instance => _settings ?? (_settings = new GameSettings());
        [JsonIgnore]
        public List<Player> Players => AvailablePlayers.Where(p => p.Enabled).ToList();
        [JsonIgnore]
        public List<Player> DefaultPlayers
           =>
               new List<Player>
               {
                    new Player("p1", "Player1", AvailableColor[0]) {Enabled = true},
                    new Player("p2", "Player2", AvailableColor[1]) {Enabled = true},
                    new Player("p3", "Player3", AvailableColor[2]),
                    new Player("p4", "Player4", AvailableColor[3])
               };

        [JsonIgnore]
        public List<ColorInformation> AvailableColor => new List<ColorInformation>()
        {
            new ColorInformation(Color.Green, "Green"),
            new ColorInformation(Color.Red, "Red"),
            new ColorInformation(Color.Blue, "Blue"),
            new ColorInformation(Color.Orange, "Orange")
        };
        #endregion


        #region Game Settings
        public List<Player> AvailablePlayers { get; set; } = new List<Player>();

        public bool FieldLines { get; set; }
        public bool WabeLines { get; set; }
        public bool BorderLines { get; set; }

        public int MaximumPlayers { get; set; }

        [JsonIgnore]
        public IPAddress Address { get; set; } = IPAddress.Loopback;
        #endregion

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

            if (AvailablePlayers.Count <= 0)
            {
                AvailablePlayers.AddRange(DefaultPlayers);
            }
            foreach (var player in AvailablePlayers.Where(p => p.Enabled))
            {
                Players.Add(player);
            }
        }

        public void LoadDefaults(FileInfo info)
        {
            if (info.DirectoryName != null) Directory.CreateDirectory(info.DirectoryName);
            FieldLines = true;
            BorderLines = true;
            WabeLines = true;
            MaximumPlayers = 2;
            if (AvailablePlayers.Count <= 0)
            {
                AvailablePlayers.AddRange(DefaultPlayers);
            }
            Save(info);
        }

        private void ApplyValues(GameSettings settings)
        {
            AvailablePlayers = settings.AvailablePlayers;
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

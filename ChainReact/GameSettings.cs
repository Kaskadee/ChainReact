using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ChainReact.Core.Game.Objects;

namespace ChainReact
{
    [Serializable]
    public class GameSettings
    {
        [XmlIgnore]
        private static GameSettings _settings;

        [XmlIgnore]
        public static GameSettings Instance => _settings ?? (_settings = new GameSettings());

        [XmlIgnore]
        public List<Player> AvailablePlayers { get; set; } = new List<Player>();

        [XmlIgnore]
        public List<Player> Players => AvailablePlayers.Where(p => p.Enabled).ToList();

        public bool FieldLines { get; set; }
        public bool WabeLines { get; set; }
        public bool BorderLines { get; set; }

        public void Save(FileInfo info)
        {
            if (!Directory.Exists(info.DirectoryName)) Directory.CreateDirectory(info.DirectoryName);
            if(!File.Exists(info.FullName)) info.Create().Close();
            
            var serializer = new XmlSerializer(typeof(GameSettings));
            using (var fs = info.OpenWrite())
            {
                fs.SetLength(0);
                serializer.Serialize(fs, this);
            }
            foreach (var player in AvailablePlayers)
            {
                player.Save();
            }
        }

        public void Load(FileInfo info, DirectoryInfo players)
        {
            if (!Directory.Exists(info.DirectoryName) || !File.Exists(info.FullName)) return;
            var serializer = new XmlSerializer(typeof(GameSettings));
            using (var fs = info.OpenRead())
            {
                var state = (GameSettings)serializer.Deserialize(fs);
                FieldLines = state.FieldLines;
                BorderLines = state.BorderLines;
                WabeLines = state.WabeLines;
            }
            foreach (var player in players.EnumerateFiles("*.sav", SearchOption.TopDirectoryOnly).Select(Player.Load))
            {
                AvailablePlayers.Add(player);
                if (player.Enabled)
                {
                    Players.Add(player);
                }
            }
        }
    }
}

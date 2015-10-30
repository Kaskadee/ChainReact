using System;
using System.Collections.Generic;
using System.IO;
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

        public List<Player> Players { get; set; }

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
                serializer.Serialize(fs, this);
            }
        }

        public void Load(FileInfo info)
        {
            if (!Directory.Exists(info.DirectoryName) || !File.Exists(info.FullName)) return;
            var serializer = new XmlSerializer(typeof(GameSettings));
            using (var fs = info.OpenRead())
            {
                var state = (GameSettings)serializer.Deserialize(fs);
                Players = state.Players;
                FieldLines = state.FieldLines;
                BorderLines = state.BorderLines;
                WabeLines = state.WabeLines;
            }
        }
    }
}

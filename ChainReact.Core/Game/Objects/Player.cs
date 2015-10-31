using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Objects
{
    [Serializable]
    public sealed class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }

        public int Score { get; set; }
        public int Wins { get; set; }

        public bool Enabled { get; set; }

        [XmlIgnore]
        public bool ExecutedFirstPlace { get; set; }
        [XmlIgnore]
        public bool Out { get; set; }

        private Player() { }

        public Player(int id, string name, Color color)
        {
            Id = id;
            Name = name;
            Color = color;
            var fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/" + Name + ".sav");
            var infos = Player.Load(fi);
            if(infos == null) Save();
        }

        public Player(int id, string name, Color color, int score, int wins)
        {
            Id = id;
            Name = name;
            Score = score;
            Wins = wins;
            Color = color;
            var fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/" + Name + ".sav");
            Save();
        }

        public string GetColorString()
        {
            if (Color.Equals(Color.Red))
            {
                return "Red";
            }
            if (Color.Equals(Color.Green))
            {
                return "Green";
            }
            if (Color.Equals(Color.Blue))
            {
                return "Blue";
            }
            if (Color.Equals(Color.Orange))
            {
                return "Orange";
            }
            return "Unknown";
        }

        public void Save()
        {
            var path = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/" + Name + ".sav");
            if (!Directory.Exists(path.DirectoryName)) Directory.CreateDirectory(path.DirectoryName);
            if(!path.Exists) path.Create().Close();
            var serializer = new XmlSerializer(typeof(Player));
            using (var fs = path.OpenWrite())
            {
                fs.SetLength(0);
                serializer.Serialize(fs, this);
            }
        }

        public static Player Load(FileInfo path)
        {
            if (!Directory.Exists(path.DirectoryName) || !path.Exists) return null;
            var serializer = new XmlSerializer(typeof(Player));
            using (var fs = path.OpenRead())
            {
                var p = (Player)serializer.Deserialize(fs);
                return p;
            }
        }
    }
}

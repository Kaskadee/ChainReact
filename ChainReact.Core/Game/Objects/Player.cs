using System;
using System.IO;
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
        }

        public Player(int id, string name, Color color, int score, int wins)
        {
            Id = id;
            Name = name;
            Score = score;
            Wins = wins;
            Color = color;
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

        public string Save(FileInfo path)
        {
            if (!Directory.Exists(path.DirectoryName)) Directory.CreateDirectory(path.DirectoryName);
            if(!path.Exists) path.Create().Close();
            var serializer = new XmlSerializer(typeof(Player));
            using (var fs = new StringWriter())
            {
                serializer.Serialize(fs, this);
                return fs.ToString();
            }
        }

        public void Load(Player p)
        {
                Id = p.Id;
                Name = p.Name;
                Color = p.Color;
                Score = p.Score;
                Wins = p.Wins;
        }
    }
}

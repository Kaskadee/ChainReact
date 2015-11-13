using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Objects
{
    [Serializable]
    public sealed class Player
    {
        private Expression<Func<Color>> _colorExpression;
        private readonly FileInfo _saveFileInfo;

        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public string ColorName { get; set; }

        public int Score { get; set; }
        public int Wins { get; set; }

        public bool Enabled { get; set; }

        [XmlIgnore]
        public bool ExecutedFirstPlace { get; set; }
        [XmlIgnore]
        public bool Out { get; set; }

        private Player()
        {
            _saveFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/Player" + Id + ".sav");
        }

        public Player(int id, string name, Expression<Func<Color>> colorExpression)
        {
            var colorFunc = colorExpression.Compile();
            ColorName = ((MemberExpression) colorExpression.Body).Member.Name;
            Color = colorFunc.Invoke();
            Id = id;
            Name = name;
            _saveFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/Player" + Id + ".sav");
        }

        public Player(int id, string name, Expression<Func<Color>> colorExpression, int score, int wins)
        {
            var colorFunc = colorExpression.Compile();
            ColorName = ((MemberExpression)colorExpression.Body).Member.Name;
            Color = colorFunc.Invoke();
            Id = id;
            Name = name;
            Score = score;
            Wins = wins;
            _saveFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/Player" + Id + ".sav");
        }

        public string GetColorString()
        {
            return !string.IsNullOrEmpty(ColorName) ? ColorName : "Unknown";
        }

        public void Save()
        {
            if (!Directory.Exists(_saveFileInfo.DirectoryName)) Directory.CreateDirectory(_saveFileInfo.DirectoryName);
            if(!_saveFileInfo.Exists) _saveFileInfo.Create().Close();
            var serializer = new XmlSerializer(typeof(Player));
            using (var fs = _saveFileInfo.OpenWrite())
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

using System;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Objects
{
    [Serializable]
    public sealed class Player
    {
        private FileInfo _saveFileInfo;

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
           
        }

        public Player(int id, string name, Expression<Func<Color>> colorExpression)
        {
            if(id == 0)
                throw new IndexOutOfRangeException("Id must be greater than 0");
            var colorFunc = colorExpression.Compile();
            ColorName = ((MemberExpression) colorExpression.Body).Member.Name;
            Color = colorFunc.Invoke();
            Id = id;
            Name = name;
            _saveFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/Player" + Id + ".sav");
        }

        public Player(int id, string name, Expression<Func<Color>> colorExpression, int score, int wins)
        {
            if (id == 0)
                throw new IndexOutOfRangeException("Id must be greater than 0");
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
            if (Id == 0)
                throw new IndexOutOfRangeException("Id must be greater than 0");

            if (_saveFileInfo.DirectoryName != null && !Directory.Exists(_saveFileInfo.DirectoryName))
            {
                Directory.CreateDirectory(_saveFileInfo.DirectoryName);
            }
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
            if (path.DirectoryName != null && !Directory.Exists(path.DirectoryName) || !path.Exists) return null;
            var serializer = new XmlSerializer(typeof(Player));
            using (var fs = path.OpenRead())
            {
                var p = (Player)serializer.Deserialize(fs);
                p.RefreshFileInfo();
                return p;
            }
        }

        public void RefreshFileInfo()
        {
            _saveFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "players/Player" + Id + ".sav");
        }
    }
}

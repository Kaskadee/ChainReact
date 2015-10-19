using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Objects
{
    public sealed class Player
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }

        public int Score { get; private set; }
        public int Wins { get; private set; }

        public bool ExecutedFirstPlace { get; set; }

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
    }
}

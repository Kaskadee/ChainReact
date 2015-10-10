using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game
{
    public class DrawRequestedEventArgs : EventArgs
    {
        public SpriteSheet SpriteSheet { get; }
        public Rectangle Position { get; }
        public Color Color { get; }
        public float Opacity { get; }

        public DrawRequestedEventArgs(SpriteSheet sheet, Rectangle position, Color color, float opacity = 1.0f)
        {
            SpriteSheet = sheet;
            Position = position;
            Color = color;
            Opacity = opacity;
        }
    }
}

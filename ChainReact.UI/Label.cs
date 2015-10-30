using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainReact.Core;
using ChainReact.UI.Base;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.UI
{
    public class Label : Control
    {
        public Label(Game game, ElementManager elementManager, string text, string font, Color color) : base(game, elementManager)
        {
            Text = text;
            Font = ResourceManager.Instance.GetResource<SpriteFont>(font);
            Color = color;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Text, Font, Position, Color);
            base.Draw(spriteBatch, gameTime);
        }
    }
}

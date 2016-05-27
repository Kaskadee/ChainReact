using ChainReact.Core;
using ChainReact.UI.Base;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.UI
{
    public class Label : Control
    {
        public Label(Game game, string text, string font, Color color) : base(game)
        {
            Text = text;
            Font = ResourceManager.GetResource<SpriteFont>(font);
            Color = color;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible) return;
            spriteBatch.DrawString(Text, Font, Position, Color);
            base.Draw(spriteBatch, gameTime);
        }
    }
}

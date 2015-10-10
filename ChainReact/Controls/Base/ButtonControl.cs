using System;
using System.Linq;
using ChainReact.Controls.Base.Interfaces;
using ChainReact.Scenes;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Controls.Base
{
    public abstract class ButtonControl : Control, IClickableControl
    {
        public bool Clicked { get; protected set; }

        private readonly SpriteFont _textFont;

        private readonly Texture2D _normalTexture;
        private readonly Texture2D _hoveredTexture;

        protected ButtonControl(Game game, ElementManager elementManager, string button, string hovered, string font)
            : base(game, elementManager)
        {
            _textFont = game.Content.Load<SpriteFont>(font);
            _normalTexture = game.Content.Load<Texture2D>(button);
            _hoveredTexture = game.Content.Load<Texture2D>(hovered);
        }

        protected ButtonControl(Game game, ElementManager elementManager, Texture2D button, Texture2D hovered, SpriteFont font) 
            : base(game, elementManager)
        {
            _textFont = font;
            _normalTexture = button;
            _hoveredTexture = hovered;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime time)
        {
            var size = _textFont.MeasureString(Text);

            spriteBatch.DrawTexture(IsHovered ? _hoveredTexture : _normalTexture,
                new Rectangle(Position.X, Position.Y, Size.Width, Size.Height));
            spriteBatch.DrawString(Text, _textFont,
                new Vector2(Position.X + ((Size.Width - size.X)/2), Position.Y + ((Size.Height - size.Y)/2)),
                Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            Clicked = false;
            base.Update(gameTime);
        }

        public virtual void OnClick(GameTime time)
        {
            Clicked = true;
        }
    }
}

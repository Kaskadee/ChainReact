using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainReact.Core;
using ChainReact.UI.Base;
using ChainReact.UI.Types;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.UI
{
    public class Button : Control, IClickableControl
    {
        public bool IsClicked { get;  set; }

        public event EventHandler OnClick;

        private readonly Texture2D _texture;
        private readonly Texture2D _hoveredTexture;

        public Button(Game game, ElementManager elementManager, string button, string hovered, string font) : base(game, elementManager)
        {
            Font = ResourceManager.Instance.GetResource<SpriteFont>(font);
            _texture = ResourceManager.Instance.GetResource<Texture2D>(button);
            _hoveredTexture = ResourceManager.Instance.GetResource<Texture2D>(hovered);
        }

        public override void Update(GameTime gameTime)
        {
            IsClicked = false;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var size = Font.MeasureString(Text);

            spriteBatch.DrawTexture(IsHovered ? _hoveredTexture : _texture,
                 new Rectangle(Position.X, Position.Y, Size.Width, Size.Height));
            spriteBatch.DrawString(Text, Font,
                new Vector2(Position.X + ((Size.Width - size.X) / 2), Position.Y + ((Size.Height - size.Y) / 2)),
                Color.White);
            base.Draw(spriteBatch, gameTime);
        }

        public void Clicked(GameTime time)
        {
            IsClicked = true;
            OnClick?.Invoke(this, EventArgs.Empty);
        }
    }
}

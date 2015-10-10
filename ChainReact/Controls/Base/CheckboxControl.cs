using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainReact.Controls.Base.Interfaces;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Controls.Base
{
    public abstract class CheckboxControl : Control, ICheckableControl
    {
        public Texture2D[] Textures { get; protected set; }
        public bool Checked { get; set; }

        protected CheckboxControl(Game game, ElementManager elementManager, Texture2D checkedTex, Texture2D uncheckedTex) : base(game, elementManager)
        {
            Textures = new Texture2D[2];
            Textures[0] = checkedTex;
            Textures[1] = uncheckedTex;
        }

        protected CheckboxControl(Game game, ElementManager elementManager, Texture2D checkedTex, Texture2D uncheckedTex, string text, SpriteFont font) : base(game, elementManager)
        {
            Textures = new Texture2D[2];
            Textures[0] = checkedTex;
            Textures[1] = uncheckedTex;
            Text = text;
            Font = font;
        }

        protected CheckboxControl(Game game, ElementManager elementManager, string checkedTex, string uncheckedTex, string text, string font) : base(game, elementManager)
        {
            Textures = new Texture2D[2];
            Textures[0] = game.Content.Load<Texture2D>(checkedTex);
            Textures[1] = game.Content.Load<Texture2D>(uncheckedTex);
            Text = text;
            Font = game.Content.Load<SpriteFont>(font);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime time)
        {
            var tex = (Checked) ? Textures[0] : Textures[1];
            var size = Font.MeasureString(Text);
            spriteBatch.DrawTexture(tex, Bounds, Color.White);
            spriteBatch.DrawString(Text, Font, new Vector2(Position.X + 35, Position.Y + 5), Color);
        }

        public virtual void OnCheckedChanged(GameTime time)
        {
            Checked = !Checked;
        }
    }
}

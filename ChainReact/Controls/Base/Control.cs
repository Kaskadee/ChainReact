using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Controls.Base
{
    public abstract class Control : Element
    {
        private MouseState _currentMouseState;
        private Rectangle _mouseRectangle;

        public Game Game { get; protected set; }

        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Size { get; set; }
        public bool IsHovered { get; private set; }

        protected Control(Game game, ElementManager elementManager)
        {
            Game = game;
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Bounds.X, Bounds.Y);
            Size = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            _currentMouseState = Mouse.GetState();
            _mouseRectangle.X = _currentMouseState.Position.X;
            _mouseRectangle.Y = _currentMouseState.Position.Y;
            IsHovered = _mouseRectangle.Intersects(Bounds);
            base.Update(gameTime);
        }
    }
}

using System;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.UI.Base
{
    public abstract class Control : Element
    {
        public event EventHandler<KeyPressEventArgs> OnKeyPress;

        private MouseState _currentMouseState;
        private Rectangle _mouseRectangle;

        public Game Game { get; protected set; }

        public string Tag { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Size { get; set; }
        public bool IsHovered { get; set; }

        [Obsolete("Use Position and Size instead of Bounds", true)]
        public new Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position.X, Position.Y, Size.Width, Size.Height);
            }
            set
            {
                Position = new Vector2(value.X, value.Y);
                Size = new Rectangle(0, 0, value.Width, value.Height);
            }
        }

        protected Control(Game game)
        {
            Game = game;
        }

        public override void InputStateReceived(InputState inputState)
        {
            if (inputState.Is<KeyboardState>())
            {
                OnKeyPress?.Invoke(this,  new KeyPressEventArgs((KeyboardState)inputState.State));   
            }
            base.InputStateReceived(inputState);
        }

        public override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();
            _mouseRectangle.X = _currentMouseState.Position.X;
            _mouseRectangle.Y = _currentMouseState.Position.Y;
            IsHovered = _mouseRectangle.Intersects(new Rectangle(Position.X, Position.Y, Size.Width, Size.Height));
            base.Update(gameTime);
        }
    }

    public class KeyPressEventArgs : EventArgs
    {
        public KeyboardState State { get; set; }

        public KeyPressEventArgs(KeyboardState state)
        {
            State = state;
        }
    }
}

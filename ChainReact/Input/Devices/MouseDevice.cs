using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChainReact
{
	public class MouseDevice : GameComponent
	{
		public MouseState MouseState { get; private set; }
		public MouseState LastMouseState { get; private set; }

		public MouseDevice (Game game) : base(game)
		{
			MouseState = Mouse.GetState ();
		}

		public override void Update (GameTime gameTime)
		{
			LastMouseState = MouseState;
			MouseState = Mouse.GetState ();
			base.Update (gameTime);
		}

		public bool LeftClick() {
			return (MouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released);
		}

		public bool LeftClickReleased() {
			return (MouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed);
		}

		public bool RightClick() {
			return (MouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released);
		}

		public bool RightClickReleased() {
			return (MouseState.RightButton == ButtonState.Released && LastMouseState.RightButton == ButtonState.Pressed);
		}
	}
}


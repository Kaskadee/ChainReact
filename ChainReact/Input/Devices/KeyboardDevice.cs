using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ChainReact
{
	public class KeyboardDevice : GameComponent
	{
		public KeyboardState KeyboardState { get; private set; }
		public KeyboardState LastKeyboardState { get; private set; }

		public KeyboardDevice (Game game) : base(game)
		{
			KeyboardState = Keyboard.GetState ();
		}

		public override void Update (GameTime gameTime)
		{
			LastKeyboardState = KeyboardState;
			KeyboardState = Keyboard.GetState ();
			base.Update (gameTime);
		}

		public bool KeyPressed(Keys key) {
			return (KeyboardState.IsKeyDown (key) && LastKeyboardState.IsKeyUp (key));
		}

		public bool KeyReleased(Keys key) {
			return (KeyboardState.IsKeyUp (key) && LastKeyboardState.IsKeyDown (key));
		}

		public bool KeyDown(Keys key) {
			return KeyboardState.IsKeyDown (key);
		}

	}
}


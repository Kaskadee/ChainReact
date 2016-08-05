using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChainReact
{
	public class InputManager : GameComponent
	{
		public Point MousePosition { get; private set; }
		public bool Clicked { get; private set; }
		public bool Menu { get; private set; }
		public bool Reset { get; private set; }

		private KeyboardDevice _keyboard;
		private MouseDevice _mouse;

		public InputManager (Game game) : base(game)
		{
			_keyboard = new KeyboardDevice (game);
			_mouse = new MouseDevice (game);
		}

		public override void Update (GameTime gameTime)
		{
			_keyboard.Update (gameTime);
			_mouse.Update (gameTime);
			MousePosition = _mouse.MouseState.Position;
			Clicked = _mouse.LeftClickReleased();
			Menu = _keyboard.KeyReleased (Keys.Escape);
			Reset = _keyboard.KeyReleased (Keys.R);
		}
	}
}


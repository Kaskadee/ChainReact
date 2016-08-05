using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ChainReact.Core.Game.Animations.Base
{
	public interface IAnimation
	{
		Texture2D Sheet { get; }
		Vector2 Position { get; }
	}
}


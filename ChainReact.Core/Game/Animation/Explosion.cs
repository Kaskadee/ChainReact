using System;
using ChainReact.Core.Game.Animations.Base;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ChainReact.Core
{
	public class Explosion : IAnimation
	{
		public Texture2D Sheet { get; private set; }
		public Vector2 Position { get; private set; }

		public bool IsFinished => (_frameIndex >= _totalFrames);

		private float _time;
		private float _frameTime = 0.04f;
		private int _frameIndex;

		private const int _totalFrames = 12;

		private int frameHeight = 134;
		private int frameWidth = 134;

		public Explosion (Texture2D sheet, Vector2 pos)
		{
			Sheet = sheet;
			Position = pos;
		}

		public void Update(GameTime time) {
			_time += (float)time.ElapsedGameTime.TotalSeconds;
			while (_time > _frameTime) {
				_frameIndex++;
				_time = 0f;
			}
		}

		public void Draw(SpriteBatch batch, GameTime time, Vector2 currentPosition) {
			var source = new Rectangle (_frameIndex * frameWidth, 0, frameWidth, frameHeight);
			var dest = new Rectangle ((int)currentPosition.X, (int)currentPosition.Y, 32, 32);
			var origin = new Vector2 (frameWidth / 2.0f, frameHeight);

			batch.Draw (Sheet, dest, source, Color.White);
		}

		public void Reset() {
			_frameIndex = 0;
			_time = 0f;
		}
	}
}


using System;
using ChainReact.Core.Game.Animations.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace ChainReact.Core
{
	public class ExplosionManager : IAnimationManager<Explosion>
	{
		private int _completedLoops;

		public Vector2 AbsolutePosition { get; set; }
		public SoundEffectInstance Sound { get; set; }
		public List<Explosion> Animations { get; }
		public bool IsRelative { get; set; } = true;

		public bool IsRunning { get; private set; }
		public int MaxLoops { get; }
		public bool AllFinished { get; private set; }

		public ExplosionManager(List<Explosion> animations, int loops, SoundEffect effect)
		{
			if (effect != null && effect.IsDisposed)
				throw new ObjectDisposedException (nameof (effect));
			if (effect != null && ResourceManager.SoundAvailable)
			{
				Sound = effect.CreateInstance ();
			}
			Animations = animations;
			MaxLoops = loops;
		}

		public void Start(out string soundError)
		{
			soundError = null;
			IsRunning = true;
			if (Sound != null && ResourceManager.SoundAvailable && Sound.State != SoundState.Playing)
			{
				try
				{
					Sound.Play();
				}
				catch (Exception ex)
				{
					soundError = ex.Message;
				}
			}
		}

		public void Stop()
		{
			IsRunning = false;
			_completedLoops = 0;
			ResetAnimations();
		}

		public void Reset()
		{
			Animations.Clear();
			_completedLoops = 0;
			IsRunning = false;
			AllFinished = false;
		}

		public void Update(GameTime gameTime)
		{
			if (!IsRunning) return;
			if (AllFinished)
			{
				throw new InvalidOperationException("ExplosionManager must be resetted to use it again");
			}
			foreach (var ani in Animations)
			{
				ani.Update (gameTime);
			}
			if (Animations.All(x => x.IsFinished))
			{
				if (_completedLoops >= MaxLoops - 1)
				{
					Stop();
					AllFinished = true;
					return;
				}
				_completedLoops++;
				ResetAnimations();
			}
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (!IsRunning) return;
			if (AllFinished)
			{
				throw new InvalidOperationException("ExplosionManager must be resetted to use it again");
			}
			foreach (var ani in Animations)
			{
				var pos = (!IsRelative) ? ani.Position : ani.Position + AbsolutePosition;
				ani.Draw (spriteBatch, gameTime, pos);
			}
		}

		public Explosion CreateNew(Vector2 rect, bool add)
		{
			var tex = ResourceManager.GetResource<Texture2D> ("Explosion");
			var explosion = new Explosion (tex, rect);
			if (add)
			{
				Animations.Add(explosion);
			}
			return explosion;
		}

		private void ResetAnimations()
		{
			foreach (var ani in Animations)
			{
				ani.Reset ();
			}
		}
	}
}


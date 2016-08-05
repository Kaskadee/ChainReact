using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ChainReact.Core.Game.Animations.Base
{
	public interface IAnimationManager<T> where T : IAnimation
	{
		List<T> Animations { get; }

		int MaxLoops { get; }
		bool AllFinished { get; }
		bool IsRunning { get; }

		void Start(out string error);
		void Stop();
		void Reset();

		T CreateNew(Vector2 rect, bool add);
	}
}


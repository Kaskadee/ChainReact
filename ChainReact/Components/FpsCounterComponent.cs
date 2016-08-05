using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace ChainReact
{
	public class FpsCounterComponent : IUpdateable
	{
		public event EventHandler<EventArgs> EnabledChanged;

		public event EventHandler<EventArgs> UpdateOrderChanged;

		public bool Enabled { get; set; }
		public int UpdateOrder { get; set; }

		public long TotalFrames { get; private set; }
		public float TotalSeconds { get; private set; }
		public float AverageFramesPerSecond { get; private set; }
		public float CurrentFramesPerSecond { get; private set; }

		public const int MAXIMUM_SAMPLES = 100;

		private Queue<float> _sampleBuffer = new Queue<float>();

		public void Update(GameTime time) 
		{
			var deltaTime = (float)time.ElapsedGameTime.TotalSeconds;
			CurrentFramesPerSecond = 1.0f / deltaTime;
			_sampleBuffer.Enqueue (CurrentFramesPerSecond);
			if (_sampleBuffer.Count > MAXIMUM_SAMPLES) {
				_sampleBuffer.Dequeue ();
				AverageFramesPerSecond = _sampleBuffer.Average (i => i);
			} else {
				AverageFramesPerSecond = CurrentFramesPerSecond;
			}

			TotalFrames++;
			TotalSeconds += deltaTime;
		}
	}
}


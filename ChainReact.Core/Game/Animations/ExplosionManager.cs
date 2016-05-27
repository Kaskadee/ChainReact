using System;
using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Game.Animations.Base;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations
{
    public sealed class ExplosionManager : IAnimationManager<Explosion>
    {
        private int _completedLoops;

        public Vector2 AbsolutePosition { get; set; }
        public SoundEffect Sound { get; set; }
        public List<Explosion> Animations { get; }
        public bool IsRelative { get; set; } = true;

        public bool IsRunning { get; private set; }
        public int MaxLoops { get; }
        public bool AllFinished { get; private set; }

        public ExplosionManager(List<Explosion> animations, int loops, SoundEffect effect)
        {
            if (effect != null && ResourceManager.SoundAvailable)
            {
                Sound = effect;
                Sound.Initialize();
            }
            Animations = animations;
            MaxLoops = loops;
        }

        public void Start(out string soundError)
        {
            soundError = null;
            IsRunning = true;
            if (Sound != null && ResourceManager.SoundAvailable && Sound.PlaybackState != PlaybackState.Playing)
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
                ani.Sheet.Update(gameTime);
            }
            if (Animations.All(x => x.Sheet.IsFinished))
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
                spriteBatch.DrawTexture(ani.Sheet, pos, Color.White);
            }
        }

        public Explosion CreateNew(Rectangle rect, bool add)
        {
            var explosion = Explosion.CreateNew(rect);
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
                ani.Sheet.AutoUpdate = true;
                ani.Sheet.Loop = false;
                ani.Sheet.ActivateKeyframe(0);
            }
        }
    }
}

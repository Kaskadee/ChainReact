using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations
{
    public class MultiAnimation : IUpdateable, IDrawable
    {
        private Sharpex2D.Framework.Game _game;
        private int _loops;

        public SoundPlayer Sound { get; set; }
        public List<Animation> Animations { get; } 
        public int Loops { get; }
        public Vector2 AbsolutePosition { get; set; }
        public bool IsRelative { get; set; } = true;
        public bool IsRunning { get; private set; }
        public bool AllFinished { get; private set; }

        public MultiAnimation(Sharpex2D.Framework.Game game, IEnumerable<Animation> animations, int loops)
        {
            _game = game;
            Animations = animations.ToList();
            Loops = loops;
            foreach (var ani in Animations)
            {
                ani.AnimationSheet.AutoUpdate = true;
                ani.AnimationSheet.Loop = false;
            }
        }

        public void Update(GameTime time)
        {
            if (Animations.All(x => x.AnimationSheet.IsFinished))
            {
                if (_loops >= Loops - 1)
                {
                    Stop();
                    AllFinished = true;
                    return;
                }
                _loops++;
                foreach (var ani in Animations)
                {
                    ani.AnimationSheet.ActivateKeyframe(0);
                    ani.AnimationSheet.Update(time);
                }
                    
            }
            if (!IsRunning) return;
            foreach (var ani in Animations)
            {
                ani.AnimationSheet.Update(time);
            }
        }

        public void Draw(SpriteBatch batch, GameTime time)
        {
            if (!IsRunning) return;
            batch.Begin();
            foreach (var ani in Animations)
            {
                var pos = (!IsRelative) ? ani.Position : ani.Position + AbsolutePosition;
                batch.DrawTexture(ani.AnimationSheet, pos, Color.White);
            }
            batch.End();
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Reset()
        {
            Stop();
            _loops = 0;
            AllFinished = false;
            foreach (var ani in Animations)
            {
                ani.AnimationSheet.ActivateKeyframe(0);
            }
               
        }
    }

    public class Animation
    {
        public AnimatedSpriteSheet AnimationSheet { get; }
        public Rectangle Position { get; set; }

        public Animation(AnimatedSpriteSheet sheet, Rectangle position)
        {
            AnimationSheet = sheet;
            Position = position;
        }
    }
}

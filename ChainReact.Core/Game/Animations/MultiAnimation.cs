using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChainReact.Core.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations
{
    public class MultiAnimation : IUpdateable, IDrawable
    {
        private Sharpex2D.Framework.Game _game;
        private int _loops;

        public SoundEffect Sound { get; set; }
        public List<Animation> Animations { get; } 
        public int Loops { get; }
        public Vector2 AbsolutePosition { get; set; }
        public bool IsRelative { get; set; } = true;
        public bool IsRunning { get; private set; }
        public bool AllFinished { get; private set; }

        public MultiAnimation(Sharpex2D.Framework.Game game, IEnumerable<Animation> animations, int loops, SoundEffect effect)
        {
            Sound = effect;
            Sound.Initialize();
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
        private readonly Texture2D _asset;
        public AnimatedSpriteSheet AnimationSheet { get; }
        public Rectangle Position { get; set; }

        public Animation(Texture2D sheet, Rectangle position)
        {
            _asset = sheet;
            AnimationSheet = new AnimatedSpriteSheet(_asset);
            for (var i = 0; i < 12; i++)
            {
                var x = 134 * i;
                var kf = new Keyframe(new Rectangle(x, 0, 134, 134), 60f);
                AnimationSheet.Add(kf);
            }
            AnimationSheet.Rectangle = new Rectangle(0, 0, 134, 134);
            AnimationSheet.AutoUpdate = true;
            Position = position;
        }

        public Animation CopyFromAnimation()
        {
            return new Animation(_asset, Position);
        }
    }
}

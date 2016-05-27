using System;
using ChainReact.Core.Game.Animations.Base;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations
{
    [Serializable]
    public class Explosion : IAnimation
    {
        public AnimatedSpriteSheet Sheet { get; }
        public Rectangle Position { get; }

        public bool CreationRequired => Sheet == null;

        private Explosion(Texture2D tex, Rectangle position, Rectangle cut)
        {
            if (tex == null)
            {
                Position = position;
                return;
            }
            Sheet = new AnimatedSpriteSheet(tex)
            {
                Rectangle = new Rectangle(0, 0, 30, 30),
                AutoUpdate = true
            };
            Position = position;

            for (var i = 0; i < 12; i++)
            {
                cut.X = 134 * i;
                var kf = new Keyframe(cut, 60f);
                Sheet.Add(kf);
            }
        }

        public static Explosion CreateNew(Rectangle pos, bool createLater = false)
        {
            if (createLater)
            {
                return new Explosion(null, pos, Rectangle.Empty);
            }
            var explosionTex = ResourceManager.GetResource<Texture2D>("ExplosionTexture");
            var cut = new Rectangle(0, 0, 134, 134);
            var explosion = new Explosion(explosionTex, pos, cut);
            return explosion;
        }

    }
}

using ChainReact.Core.Game.Animations.Base;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations
{
    public class Explosion : IAnimation
    {
        public AnimatedSpriteSheet Sheet { get; }
        public Rectangle Position { get; }

        private Explosion(Texture2D tex, Rectangle position, Rectangle cut)
        {
            Sheet = new AnimatedSpriteSheet(tex)
            {
                Rectangle = new Rectangle(0, 0, 32, 32),
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

        public static Explosion CreateNew(Rectangle pos)
        {
            var explosionTex = ResourceManager.Instance.GetResource<Texture2D>("Explosion");
            var cut = new Rectangle(0, 0, 134, 134);
            var explosion = new Explosion(explosionTex, pos, cut);
            return explosion;
        }

    }
}

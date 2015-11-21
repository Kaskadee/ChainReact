using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations.Base
{
    public interface IAnimation
    {
        AnimatedSpriteSheet Sheet { get; }
        Rectangle Position { get; }
    }
}

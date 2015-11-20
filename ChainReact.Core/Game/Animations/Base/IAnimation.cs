using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Game.Animations
{
    public interface IAnimation
    {
        AnimatedSpriteSheet Sheet { get; }
        Rectangle Position { get; }
    }
}

using System.Collections.Generic;
using Sharpex2D.Framework;

namespace ChainReact.Core.Game.Animations.Base
{
    public interface IAnimationManager<T> : IUpdateable, IDrawable where T : IAnimation
    {
        List<T> Animations { get; }

        int MaxLoops { get; }
        bool AllFinished { get; }
        bool IsRunning { get; }

        void Start(out string error);
        void Stop();
        void Reset();

        T CreateNew(Rectangle rect, bool add, bool createLater);
    }
}

using ChainReact.Utilities;
using Sharpex2D.Framework;

namespace ChainReact.Input
{
    public interface IInputController
    {
        int Priority { get; }

        Vector2 Position { get; }
        Trigger Clicked { get; }
        Trigger Reset { get; }
        Trigger Menu { get; }

        void Update(GameTime time);
    }
}

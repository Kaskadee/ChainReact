using ChainReact.Core.Utilities;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.UI;

namespace ChainReact.Input
{
    public interface IInputController
    {
        int Priority { get; }

        Vector2 Position { get; }
        Trigger Clicked { get; }
        Trigger Reset { get; }
        Trigger Menu { get; }

        InputState State { get; }

        void Update(GameTime time);
    }
}

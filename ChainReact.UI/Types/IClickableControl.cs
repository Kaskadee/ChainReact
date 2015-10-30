using Sharpex2D.Framework;

namespace ChainReact.UI.Types
{
    public interface IClickableControl
    {
        bool IsClicked { get; }
        void Clicked(GameTime time);
    }
}

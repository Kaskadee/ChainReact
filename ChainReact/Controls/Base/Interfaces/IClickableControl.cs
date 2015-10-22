using Sharpex2D.Framework;

namespace ChainReact.Controls.Base.Interfaces
{
    public interface IClickableControl
    {
        bool Clicked { get; }
        void OnClick(GameTime time);
    }
}

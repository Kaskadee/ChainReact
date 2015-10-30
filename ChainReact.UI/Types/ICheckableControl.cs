using Sharpex2D.Framework;

namespace ChainReact.UI.Types
{
    public interface ICheckableControl
    {
        bool IsChecked { get; }
        void Checked(GameTime time);
    }
}

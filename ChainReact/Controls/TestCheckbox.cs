using ChainReact.Controls.Base;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Controls
{
    public sealed class TestCheckbox : CheckboxControl
    {
        public TestCheckbox(Game game, ElementManager elementManager, Texture2D checkedTex, Texture2D uncheckedTex) : base(game, elementManager, checkedTex, uncheckedTex)
        {
            Position = new Vector2(300, 300);
        }

        public TestCheckbox(Game game, ElementManager elementManager, Texture2D checkedTex, Texture2D uncheckedTex, string text, SpriteFont font) : base(game, elementManager, checkedTex, uncheckedTex, text, font)
        {
            Position = new Vector2(300, 300);
        }

        public TestCheckbox(Game game, ElementManager elementManager, string checkedTex, string uncheckedTex, string text, string font) : base(game, elementManager, checkedTex, uncheckedTex, text, font)
        {
            Position = new Vector2(300, 300);
        }
    }
}

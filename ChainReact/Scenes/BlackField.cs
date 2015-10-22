using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Scenes
{
    public static class BlackField
    {
        private static Texture2D _blackField;

        public static void DrawField(Game game, SpriteBatch batch, float opacity = 0f)
        {
            if(_blackField == null) LoadTexture();
            batch.DrawTexture(_blackField, new Rectangle(0, 0, game.Window.ClientSize.X, game.Window.ClientSize.Y), Color.White, opacity);
        }

        private static void LoadTexture()
        {
            _blackField = ColorTextureConverter.CreateTextureFromColor(32, 32, Color.Black);
        }
    }
}

using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Scenes
{
    public class Coverage
    {
        private readonly Color _col;
        private Texture2D _coverage;

        public Coverage(Color col)
        {
            _col = col;
        }

        public void DrawField(Game game, SpriteBatch batch, float opacity = 0f)
        {
            if(_coverage == null) LoadTexture(_col);
            batch.DrawTexture(_coverage, new Rectangle(0, 0, game.Window.ClientSize.X, game.Window.ClientSize.Y), Color.White, opacity);
        }

        private void LoadTexture(Color col)
        {
            _coverage = ColorTextureConverter.CreateTextureFromColor(32, 32, col);
        }
    }
}

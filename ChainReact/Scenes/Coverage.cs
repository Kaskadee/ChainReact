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

        public void DrawField(Game game, SpriteBatch batch, byte opacity = 255)
        {
            if(_coverage == null) LoadTexture(_col);
            var col = Color.White;
            col.A = opacity;
            batch.DrawTexture(_coverage, new Rectangle(0, 0, game.Window.ClientSize.X, game.Window.ClientSize.Y), col);
        }

        private void LoadTexture(Color col)
        {
            _coverage = ColorTextureConverter.CreateTextureFromColor(32, 32, col);
        }
    }
}

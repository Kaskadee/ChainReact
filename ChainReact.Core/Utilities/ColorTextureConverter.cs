using Sharpex2D.Framework.Rendering;

namespace ChainReact.Core.Utilities
{
    public class ColorTextureConverter
    {
        public static Texture2D CreateTextureFromColor(int width, int heigth, Color color)
        {
            var tex = new Texture2D(width, heigth);
            tex.Lock();
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < heigth; y++)
                {
                    tex[x, y] = color;
                }
            }
            tex.Unlock();
            return tex;
        }
    }
}

using Sharpex2D.Framework.Rendering;

namespace ChainReact.Utilities
{
    public static class TextureUtilities
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

        public static Texture2D CreateBorderFromColor(int width, int heigth, int penLength, Color color)
        {
            var tex = new Texture2D(width, heigth);
            tex.Lock();
            for (var x = 0; x <= width - 1; x++)
            {
                for (var y = 0; y <= heigth - 1; y++)
                    if ((penLength - x) > 0 || (penLength + x) > width - 1)
                        tex[x, y] = color;
            }

            for (var y = 0; y <= heigth - 1; y++)
            {
                for (var x = 0; x <= width - 1; x++)
                    if ((penLength - y) > 0 || (penLength + y) > heigth - 1)
                        tex[x, y] = color;
            }

            tex.Unlock();
            return tex;
        }
    }
}

using Sharpex2D.Framework.Rendering;

namespace ChainReact.UI.Extensions
{
    public static class Texture2DExtension
    {
        public static Texture2D Grayscale(this Texture2D tex)
        {
            var newTex = new Texture2D(tex.Width, tex.Height);
            newTex.Lock();
            tex.Lock();
            for (var x = 0; x < tex.Width; x++)
            {
                for (var y = 0; y < tex.Height; y++)
                {
                    var orginalColor = tex[x, y];
                    var grayscale = (int) ((orginalColor.R*0.3) + (orginalColor.G*0.59) + (orginalColor.B*0.11));
                    var newColor = Color.FromArgb(orginalColor.A, grayscale, grayscale, grayscale);
                    newTex[x, y] = newColor;
                }
            }
            tex.Unlock();
            newTex.Unlock();
            return newTex;
        }
    }
}

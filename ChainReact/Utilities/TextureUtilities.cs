using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ChainReact.Utilities
{
	public static class TextureUtilities
	{
		public static Texture2D ConvertFromColor(GraphicsDevice device, Color col) {
			var tex = new Texture2D (device, 32, 32);
			var data = new Color[32 * 32];
			for (var i = 0; i < 1024; i++) {
				data [i] = col;
			}
			tex.SetData (data);
			return tex;
		}

		public static Texture2D CreateBorderFromColor(GraphicsDevice device, int width, int height, int penLength, Color col) {
			var tex = new Texture2D (device, width, height);
			var data = new Color[width,height];
			for (var x = 0; x <= width - 1; x++)
			{
				for (var y = 0; y <= height - 1; y++)
					if ((penLength - x) > 0 || (penLength + x) > width - 1)
						data[x, y] = col;
			}

			for (var y = 0; y <= height - 1; y++)
			{
				for (var x = 0; x <= width - 1; x++)
					if ((penLength - y) > 0 || (penLength + y) > height - 1)
						data[x, y] = col;
			}
			var oneDimensional = data.Cast<Color> ().ToArray ();
			tex.SetData (oneDimensional);
			return tex;

		}
	}
}


using System;
using Microsoft.Xna.Framework;

namespace ChainReact.Core.Rendering
{
	public class ColorInformation
	{
		public Color Color { get; set; }
		public string ColorName { get; set; }

		public ColorInformation(Color color, string name)
		{
			Color = color;
			ColorName = name;
		}
	}
}


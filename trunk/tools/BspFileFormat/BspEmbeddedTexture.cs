using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BspFileFormat
{
	public class BspEmbeddedTexture : BspTexture
	{
		public Bitmap[] mipMaps;
	}
}

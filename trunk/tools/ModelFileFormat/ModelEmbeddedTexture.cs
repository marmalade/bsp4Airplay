using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace ModelFileFormat
{
	public class ModelEmbeddedTexture : ModelTexture
	{
		public Bitmap Bitmap;

		public ModelEmbeddedTexture(string name, Bitmap bitmap)
		{
			this.Name = name;
			this.Bitmap = bitmap;
		}
	}
}
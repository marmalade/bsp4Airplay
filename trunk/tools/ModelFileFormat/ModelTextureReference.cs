using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat
{
	public class ModelTextureReference : ModelTexture
	{
		public ModelTextureReference(string name)
		{
			this.Name = name;
		}
	}
}

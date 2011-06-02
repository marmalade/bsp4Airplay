using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat
{
	public interface IModelReader
	{
		void ReadModel(BinaryReader source, ModelDocument dest);
	}
}

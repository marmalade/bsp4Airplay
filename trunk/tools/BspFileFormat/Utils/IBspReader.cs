using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BspFileFormat.Utils
{
	public interface IBspReader
	{
		void ReadBsp(BinaryReader source, BspDocument dest);
	}
}

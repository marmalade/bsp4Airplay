using System;
using System.Collections.Generic;
using System.Text;
using ReaderUtils;
using System.IO;

namespace BspFileFormat.HL2
{
	public class edge_t
	{
		public ushort vertex0;             // index of the start vertex, must be in [0,numvertices[
		public ushort vertex1;             // index of the end vertex,  must be in [0,numvertices[

		public void Read(BinaryReader source)
		{
			vertex0 = source.ReadUInt16();
			vertex1 = source.ReadUInt16();
		}
	};
}

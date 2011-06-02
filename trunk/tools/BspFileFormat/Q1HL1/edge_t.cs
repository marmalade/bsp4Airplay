using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public class edge_t
	{
		public ushort vertex0;             // index of the start vertex, must be in [0,numvertices[
		public ushort vertex1;             // index of the end vertex,  must be in [0,numvertices[

		public void Read(System.IO.BinaryReader source)
		{
			vertex0 = source.ReadUInt16();
			vertex1 = source.ReadUInt16();
		}
	}

}

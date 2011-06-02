using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public struct dentry_t
	{
		public uint offset;
		public uint size;

		internal void Read(System.IO.BinaryReader source)
		{
			offset = source.ReadUInt32();
			size = source.ReadUInt32();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BspFileFormat.Q3
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

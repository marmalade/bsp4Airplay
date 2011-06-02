using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;

namespace BspFileFormat.Q2
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

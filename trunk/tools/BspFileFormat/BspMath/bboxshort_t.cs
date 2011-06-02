using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.BspMath
{
	public struct bboxshort_t
	{
		public short[] mins;
		public short[] maxs;

		internal void Read(System.IO.BinaryReader source)
		{
			mins = new short[3];
			maxs = new short[3];
			mins[0] = source.ReadInt16();
			mins[1] = source.ReadInt16();
			mins[2] = source.ReadInt16();
			maxs[0] = source.ReadInt16();
			maxs[1] = source.ReadInt16();
			maxs[2] = source.ReadInt16();
		}
	}
	public struct bboxint_t
	{
		public int[] mins;
		public int[] maxs;

		internal void Read(System.IO.BinaryReader source)
		{
			mins = new int[3];
			maxs = new int[3];
			mins[0] = source.ReadInt32();
			mins[1] = source.ReadInt32();
			mins[2] = source.ReadInt32();
			maxs[0] = source.ReadInt32();
			maxs[1] = source.ReadInt32();
			maxs[2] = source.ReadInt32();
		}
	}
}

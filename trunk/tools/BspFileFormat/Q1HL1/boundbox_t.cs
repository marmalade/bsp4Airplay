using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public struct boundbox_t
	{
		Vector3 min;
		Vector3 max;

		public void Read(System.IO.BinaryReader source)
		{
			min.X = source.ReadSingle();
			min.Y = source.ReadSingle();
			min.Z = source.ReadSingle();
			max.X = source.ReadSingle();
			max.Y = source.ReadSingle();
			max.Z = source.ReadSingle();
		}
	}
}

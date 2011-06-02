using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.IO;
using ReaderUtils;
using System.Collections;

namespace BspFileFormat.HL2
{
	public class HL2Reader17 : HL2Reader
	{
		public override void ReadFaces(BinaryReader source)
		{
			faces = new List<face_t>();
			IList src = (ReaderHelper.ReadStructs<face_17t>(source, header.Faces.size, header.Faces.offset + startOfTheFile, 104));
			foreach (var f in src)
				((IList)faces).Add(f);
		}
		public override void ReadLeaves(BinaryReader source)
		{
			dleaves = new List<dleaf_t>();
			IList src = (ReaderHelper.ReadStructs<dleaf_17t>(source, header.Leafs.size, header.Leafs.offset + startOfTheFile, 32));
			foreach (var f in src)
				((IList)faces).Add(f);
		}
	}
}

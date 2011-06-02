using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ReaderUtils;

namespace ModelFileFormat.HL1
{
	public class mstudio_bodyparts_t
	{
		public string name;
		public int nummodels;
		public int _base;
		public int modelindex; // index into models array
		public List<mstudio_model_t> Models;
		public void Read(BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[] { '\0' });
			nummodels = source.ReadInt32();
			_base = source.ReadInt32();
			modelindex = source.ReadInt32();
		}
	}
}

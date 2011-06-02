using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public class mipheader_t               // Mip texture list header
	{
		public int numtex;                 // Number of textures in Mip Texture list
		public int[] offset;         // Offset to each of the individual texture [numtex]

		public void Read(System.IO.BinaryReader source)
		{
			numtex = source.ReadInt32();
			offset = new int[numtex];
			for (int i=0; i<numtex; ++i)
				offset[i] = source.ReadInt32();
		}
	} ;
}

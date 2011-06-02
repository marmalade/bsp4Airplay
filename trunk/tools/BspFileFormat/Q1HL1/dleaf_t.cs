using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q1HL1
{
	public class dleaf_t
	{
		public int type;                   // Special type of leaf
		public int vislist;                // Beginning of visibility lists
		//     must be -1 or in [0,numvislist[
		public bboxshort_t box;           // Bounding box of the leaf
		public ushort lface_id;            // First item of the list of faces
		//     must be in [0,numlfaces[
		public ushort lface_num;           // Number of faces in the leaf  
		public byte sndwater;             // level of the four ambient sounds:
		public byte sndsky;               //   0    is no sound
		public byte sndslime;             //   0xFF is maximum volume
		public byte sndlava;              //

		public List<int> VisibleLeaves = new List<int>();

		public void Read(System.IO.BinaryReader source)
		{
			type = source.ReadInt32();
			vislist = source.ReadInt32();
			box.Read(source);
			lface_id = source.ReadUInt16();
			lface_num = source.ReadUInt16();
			sndwater = source.ReadByte();
			sndsky = source.ReadByte();
			sndslime = source.ReadByte();
			sndlava = source.ReadByte();
		}
	} ;
}

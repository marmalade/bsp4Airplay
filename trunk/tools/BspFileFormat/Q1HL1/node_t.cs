using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q1HL1
{
	public class node_t
	{
		public int planenum;            // The plane that splits the node
		//           must be in [0,numplanes[
		public ushort front;               // If bit15==0, index of Front child node
		// If bit15==1, ~front = index of child leaf
		public ushort back;                // If bit15==0, id of Back child node
		// If bit15==1, ~back =  id of child leaf

		public bboxshort_t box;             // Bounding box of node and all childs
		public ushort face_id;             // Index of first Polygons in the node
		public ushort face_num;            // Number of faces in the node

		public void Read(System.IO.BinaryReader source)
		{
			planenum = source.ReadInt32();
			front = source.ReadUInt16();
			back = source.ReadUInt16();

			box.Read(source);
			face_id = source.ReadUInt16();
			face_num = source.ReadUInt16();
		}
	} ;
}

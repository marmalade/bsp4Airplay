using ReaderUtils;
using System.IO;
using BspFileFormat.BspMath;

namespace BspFileFormat.HL2
{
		public class dnode_t
		{
			public int         planenum;         // index into plane array
			public int         front;      // negative numbers are -(leafs+1), not nodes
			public int         back;      // negative numbers are -(leafs+1), not nodes
			public bboxshort_t box;
			public ushort    face_id;  // index into face array
			public ushort    face_num;   // counting both sides
			public short       area;             // If all leaves below this node are in the same area, then
							// this is the area index. If not, this is -1.
			public short	paddding;		// pad to 32 bytes length
			public void Read(BinaryReader source)
			{
				planenum = source.ReadInt32();
				front = source.ReadInt32();
				back = source.ReadInt32();

				box.Read(source);
				face_id = source.ReadUInt16();
				face_num = source.ReadUInt16();
				area = source.ReadInt16();
				paddding = source.ReadInt16();
			}
		};
}

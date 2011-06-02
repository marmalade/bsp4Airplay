using ReaderUtils;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q2
{
	public class node_t
	{
		public uint planenum;             // index of the splitting plane (in the plane array)

		public int front;       // index of the front child node or leaf
		public int back;        // index of the back child node or leaf

		public bboxshort_t box;

		public ushort first_face;        // index of the first face (in the face array)
		public ushort num_faces;         // number of consecutive edges (in the face array)

		public void Read(System.IO.BinaryReader source)
		{
			planenum = source.ReadUInt32();
			front = source.ReadInt32();
			back = source.ReadInt32();
			box.Read(source);
			first_face = source.ReadUInt16();
			num_faces = source.ReadUInt16();
		}

	}
}

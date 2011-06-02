using ReaderUtils;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q3
{

	public class node_t
	{
		public int planenum;
		public int front;
		public int back;
		public bboxint_t box;


		public void Read(System.IO.BinaryReader source)
		{
			planenum = source.ReadInt32();
			front = source.ReadInt32();
			back = source.ReadInt32();
			box.Read(source);
		}
	}
}

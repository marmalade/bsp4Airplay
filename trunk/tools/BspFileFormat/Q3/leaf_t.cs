using ReaderUtils;
using BspFileFormat.BspMath;

namespace BspFileFormat.Q3
{
	public class leaf_t
	{
		public int cluster;
		public int area;
		public bboxint_t box;

		public int leafface;
		public int n_leaffaces;
		public int leafbrush;
		public int n_leafbrushes;

		public void Read(System.IO.BinaryReader source)
		{
			cluster = source.ReadInt32();
			area = source.ReadInt32();
			box.Read(source);

			leafface = source.ReadInt32();
			n_leaffaces = source.ReadInt32();
			leafbrush = source.ReadInt32();
			n_leafbrushes = source.ReadInt32();
		}
	}
}

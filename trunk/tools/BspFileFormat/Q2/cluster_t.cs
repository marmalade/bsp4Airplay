using ReaderUtils;
using System.Collections.Generic;

namespace BspFileFormat.Q2
{
	public class cluster_t
	{
		public int offset;
		public int phs;
		public List<int> lists = new List<int>();
		public List<int> visiblity = new List<int>();
	}
}

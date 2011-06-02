using System.Collections.Generic;
using ReaderUtils;
namespace BspFileFormat
{
	public class BspCollisionConvexBrush : BspCollisionObject
	{
		public List<Plane> Planes = new List<Plane>();
	}
}

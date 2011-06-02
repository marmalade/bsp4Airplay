using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aCollisionConvexBrush : CIwParseable, Ib4aCollider
	{
		//public List<int> Planes = new List<int>();
		//public override void WrtieBodyToStream(CTextWriter writer)
		//{
		//    base.WrtieBodyToStream(writer);
		//    writer.WriteKeyVal("num_planes", Planes.Count);
		//    foreach (var v in Planes)
		//        writer.WriteKeyVal("plane", v);
		//}

		public List<CIwPlane> Planes = new List<CIwPlane>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_planes", Planes.Count);
			foreach (var v in Planes)
				writer.WriteArray("plane", new int[] { v.v.x, v.v.y, v.v.z, v.k });
		}
	}
}

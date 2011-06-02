using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aNode : Cb4aTreeElement
	{
		public bool IsFrontLeaf;
		public int Front;
		public bool IsBackLeaf;
		public int Back;
		public int Plane;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			//writer.WriteArray("plane", new int[]{PlaneNormal.x,(int)PlaneNormal.y,(int)PlaneNormal.z, (int)(PlaneDistance * AirplaySDKMath.IW_GEOM_ONE)});
			writer.WriteKeyVal("plane", Plane);
			writer.WriteKeyVal("is_front_leaf", IsFrontLeaf);
			writer.WriteKeyVal("front", Front);
			writer.WriteKeyVal("is_back_leaf", IsBackLeaf);
			writer.WriteKeyVal("back", Back);
		}
	}
}

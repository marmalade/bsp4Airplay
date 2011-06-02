using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwAnimBone : CIwManaged
	{
		public CIwVec3 pos;
		public CIwQuat rot;
		public string parent;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			if (!string.IsNullOrEmpty(parent))
				writer.WriteString("parent", parent);
			writer.WriteVec3("pos", pos);
			writer.WriteQuat("rot", rot);
		}
	}
}

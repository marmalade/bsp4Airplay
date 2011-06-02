using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwAnimKeyFrameBone : CIwParseable
	{
		public string bone;
		public CIwVec3 pos;
		public CIwQuat rot;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);

			writer.WriteString("bone", bone);
			writer.WriteVec3("pos", pos);
			writer.WriteQuat("rot", rot);
		}
	}
}

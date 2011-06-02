using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwAnimKeyFrame : CIwParseable
	{
		public float time;
		public List<CIwAnimKeyFrameBone> bones = new List<CIwAnimKeyFrameBone>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("time", time);

			foreach (var bone in bones)
				bone.WrtieBodyToStream(writer);
		}
	}
}

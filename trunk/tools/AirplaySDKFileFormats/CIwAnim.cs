using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwAnim : CIwResource
	{
		public CIwAnimSkel skeleton;
		public List<CIwAnimKeyFrame> KeyFrames = new List<CIwAnimKeyFrame>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteString("skeleton", skeleton.Name);
			if (KeyFrames.Count == 0)
			{
				(new CIwAnimKeyFrame()).WrtieToStream(writer);
			}
			else
			{
				foreach (var bone in KeyFrames)
					bone.WrtieToStream(writer);
			}
		}
	}
}

using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwAnimSkel : CIwResource
	{
		public List<CIwAnimBone> Bones = new List<CIwAnimBone>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);

			writer.WriteKeyVal("numBones",Bones.Count);
			foreach (var bone in Bones)
				bone.WrtieToStream(writer);
		}
	}
}

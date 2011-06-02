using AirplaySDKFileFormats.Model;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class CIwAnimSkin : CIwResource
	{
		public List<CIwAnimSkinSet> SkinSets = new List<CIwAnimSkinSet>();
		public List<CIwAnim> Animations;
		public CIwAnimSkel skeleton;
		public CIwModel model;

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			if (skeleton != null)
				writer.WriteString("skeleton", skeleton.Name);
			if (model != null)
				writer.WriteString("model", model.Name);

			foreach (var skinSet in SkinSets)
				skinSet.WrtieToStream(writer);
		}
	}
}

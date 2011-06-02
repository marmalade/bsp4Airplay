namespace AirplaySDKFileFormats
{
	public class CIwMaterial : CIwResource
	{
		public string Texture0;
		public string Texture1;
		public bool sky;
		public bool transparent;
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteString("texture0", Texture0);
			writer.WriteString("texture1", Texture1);
			//writer.WriteKeyVal("sky", sky);
			//writer.WriteKeyVal("transparent", transparent);
		}

	}
}

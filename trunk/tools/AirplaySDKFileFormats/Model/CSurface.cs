namespace AirplaySDKFileFormats.Model
{
	public class CSurface : CIwParseable
	{
		public string Material;

		public CTris Triangles = new CTris();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteString("material", Material);
			if (Triangles.Elements.Count > 0)
			{
				Triangles.WrtieToStream(writer);
			}
		}
	}
}

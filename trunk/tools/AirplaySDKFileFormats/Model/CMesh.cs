using System.Collections.Generic;
using System.Globalization;

namespace AirplaySDKFileFormats.Model
{
	/// <summary>
	/// CMesh
	/// Airplay SDK implements this through ParseMesh helper class
	/// </summary>
	public class CMesh : CIwParseable
	{
		public string Name;
		public float Scale=1;
		public CVerts Verts = new CVerts();
		public CVertNorms VertNorms = new CVertNorms();
		public CVertCols VertCols = new CVertCols();
		public IList<CUVs> UVs = new List<CUVs>();
		public IList<CSurface> Surfaces = new List<CSurface>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteString("name", Name);
			writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "scale {0}", Scale));
			Verts.WrtieToStream(writer);
			VertNorms.WrtieToStream(writer);
			VertCols.WrtieToStream(writer);
			foreach (var u in UVs)
				u.WrtieToStream(writer);
			foreach (var s in Surfaces)
				s.WrtieToStream(writer);
		}
	}
}

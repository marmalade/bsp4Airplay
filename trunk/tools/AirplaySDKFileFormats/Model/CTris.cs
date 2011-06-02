using System.Collections.Generic;
namespace AirplaySDKFileFormats.Model
{
	public class CTris : CIwParseable
	{
		public List<CTrisElement> Elements = new List<CTrisElement>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			writer.WriteKeyVal("numTris", Elements.Count);
			foreach (var t in Elements)
			{
				writer.BeginWriteLine();
				writer.Write("t");
				var v = t.Vertex0;
				writer.Write(string.Format(" {{{0},{1},{2},{3},{4}}}", v.pos,v.n,v.uv0,v.uv1,v.color));
				v = t.Vertex1;
				writer.Write(string.Format(" {{{0},{1},{2},{3},{4}}}", v.pos, v.n, v.uv0, v.uv1, v.color));
				v = t.Vertex2;
				writer.Write(string.Format(" {{{0},{1},{2},{3},{4}}}", v.pos, v.n, v.uv0, v.uv1, v.color));
				writer.EndWriteLine();
			}
		}
	}
	public struct CTrisVertex
	{
		public int pos;
		public int n;
		public int uv0;
		public int uv1;
		public int color;

		public CTrisVertex(int p)
		{
			pos = p;
			n = -1;
			uv0 = -1;
			uv1 = -1;
			color = -1;
		}
		public CTrisVertex(int p, int n, int uv0, int uv1, int color)
		{
			pos = p;
			this.n = n;
			this.uv0 = uv0;
			this.uv1 = uv1;
			this.color = color;
		}
	}
}

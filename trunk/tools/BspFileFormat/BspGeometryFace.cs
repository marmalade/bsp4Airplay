using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ReaderUtils;

namespace BspFileFormat
{
	public class BspGeometryFace
	{
		public BspGeometryVertex Vertex0;
		public BspGeometryVertex Vertex1;
		public BspGeometryVertex Vertex2;
		public BspTexture Texture;
		public BspTexture Lightmap;

		public float MaxUV0Distance
		{
			get
			{
				return Math.Max(
			   Math.Max(Math.Max(Math.Abs(Vertex0.UV0.X - Vertex1.UV0.X), Math.Abs(Vertex0.UV0.X - Vertex2.UV0.X)), Math.Abs(Vertex1.UV0.X - Vertex2.UV0.X)),
			   Math.Max(Math.Max(Math.Abs(Vertex0.UV0.Y - Vertex1.UV0.Y), Math.Abs(Vertex0.UV0.Y - Vertex2.UV0.Y)), Math.Abs(Vertex1.UV0.Y - Vertex2.UV0.Y)))
			   ;
			}
		}
	}
}

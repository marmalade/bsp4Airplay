using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ReaderUtils;

namespace BspFileFormat
{
	public class BspGeometryVertex
	{
		public Vector3 Position = Vector3.Zero;
		public Vector3 Normal = Vector3.UnitZ;
		public Vector2 UV1 = Vector2.Zero;
		public Vector2 UV0 = Vector2.Zero;
		public Color Color = Color.FromArgb(255,255,255,255);

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ UV0.GetHashCode() ^ UV1.GetHashCode() ^ Color.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BspGeometryVertex))
				return false;

			return this.Equals((BspGeometryVertex)obj);
		}

		public bool Equals(BspGeometryVertex other)
		{
			return
				Position == other.Position &&
				Normal == other.Normal &&
				UV0 == other.UV0 &&
				UV1 == other.UV1 &&
				Color == other.Color;
		}
	}
}

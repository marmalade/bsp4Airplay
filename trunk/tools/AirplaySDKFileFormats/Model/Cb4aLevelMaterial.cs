using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace AirplaySDKFileFormats.Model
{
	public class Cb4aLevelMaterial : CIwParseable
	{
		public string Texture;
		public string Lightmap;
		public bool Sky;
		public bool Transparent;
		public Cb4aLevelMaterial()
		{
		}
		public Cb4aLevelMaterial(string Texture, string Lightmap)
		{
			this.Texture = Texture;
			this.Lightmap = Lightmap;
		}
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteString("texture0", Texture);
			writer.WriteString("texture1", Lightmap);
			if (Sky)
				writer.WriteKeyVal("sky", Sky);
			if (Transparent)
				writer.WriteKeyVal("transparent", Transparent);
		}

		public override int GetHashCode()
		{
			int res = 0;
			if (Texture != null)
				res ^= Texture.GetHashCode();
			if (Lightmap != null)
				res ^= Lightmap.GetHashCode();
			return res;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Cb4aLevelMaterial))
				return false;

			return this.Equals((Cb4aLevelMaterial)obj);
		}
		public bool Equals(Cb4aLevelMaterial other)
		{
			return
				Texture == other.Texture &&
				Lightmap == other.Lightmap;
		}
		public static bool operator ==(Cb4aLevelMaterial left, Cb4aLevelMaterial right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Cb4aLevelMaterial left, Cb4aLevelMaterial right)
		{
			return !left.Equals(right);
		}
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{{{0},{1}}}", Texture, Lightmap);
		}
	}
}

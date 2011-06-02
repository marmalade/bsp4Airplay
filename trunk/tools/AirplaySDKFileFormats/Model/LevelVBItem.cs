using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats.Model
{
	public struct LevelVBItem
	{
		public CIwVec3 Position;
		public CIwVec3 Normal;
		public CIwVec2 UV0;
		public CIwVec2 UV1;
		public CIwColour Colour;

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ UV0.GetHashCode() ^ UV1.GetHashCode() ^ Colour.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is LevelVBItem))
				return false;

			return this.Equals((LevelVBItem)obj);
		}
		public bool Equals(LevelVBItem other)
		{
			return
				Position == other.Position &&
				Normal == other.Normal &&
				UV0 == other.UV0 &&
				UV1 == other.UV1 &&
				Colour == other.Colour
				;
		}
		public static bool operator ==(LevelVBItem left, LevelVBItem right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(LevelVBItem left, LevelVBItem right)
		{
			return !left.Equals(right);
		}
		//public override string ToString()
		//{
		//    return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2}}}", x, y, z);
		//}

		internal void WrtieToStream(CTextWriter writer)
		{
			writer.WriteVec3("v", Position);
			writer.WriteVec3("vn", Normal);
			writer.WriteVec2("uv0", UV0);
			writer.WriteVec2("uv1", UV1);
			writer.WriteColour("col", Colour);
		}
	}
}

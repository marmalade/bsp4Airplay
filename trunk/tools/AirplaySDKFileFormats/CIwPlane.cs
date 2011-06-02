using System;
using System.Globalization;

namespace AirplaySDKFileFormats
{
	public struct CIwPlane
	{
		public CIwVec3 v;
		public int k;

		public override int GetHashCode()
		{
			return v.GetHashCode() ^ k.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is CIwPlane))
				return false;

			return this.Equals((CIwPlane)obj);
		}
		public bool Equals(CIwPlane other)
		{
			return
				v == other.v &&
				k == other.k;
		}
		public static bool operator ==(CIwPlane left, CIwPlane right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(CIwPlane left, CIwPlane right)
		{
			return !left.Equals(right);
		}
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2},{3}}}", v.x, v.y, v.z, k);
		}
	}
}

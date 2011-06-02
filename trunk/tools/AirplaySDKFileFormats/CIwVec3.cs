using System;
using System.Globalization;
namespace AirplaySDKFileFormats
{
	public struct CIwVec3
	{
		public int x;
		public int y;
		public int z;

		public static CIwVec3 g_Zero = new CIwVec3(0, 0, 0);
	
		public CIwVec3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public static CIwVec3 operator -(CIwVec3 left, CIwVec3 right)
		{
			left.x -= right.x;
			left.y -= right.y;
			left.z -= right.z;
			return left;
		}
		public static CIwVec3 operator +(CIwVec3 left, CIwVec3 right)
		{
			left.x += right.x;
			left.y += right.y;
			left.z += right.z;
			return left;
		}
		public int Length
		{
			get 
			{
				return (int)Math.Sqrt((float)x * (float)x + (float)y * (float)y + (float)z * (float)z);
			}
		}
		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is CIwVec3))
				return false;

			return this.Equals((CIwVec3)obj);
		}
		public bool Equals(CIwVec3 other)
		{
			return
				x == other.x &&
				y == other.y &&
				z == other.z;
		}
		public static bool operator ==(CIwVec3 left, CIwVec3 right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(CIwVec3 left, CIwVec3 right)
		{
			return !left.Equals(right);
		}
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2}}}", x, y, z);
		}
	}
}

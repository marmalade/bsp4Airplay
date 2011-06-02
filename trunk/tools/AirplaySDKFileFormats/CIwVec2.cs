using System;
using System.Globalization;
namespace AirplaySDKFileFormats
{
		public struct CIwVec2
			{
				public int x;
				public int y;

				public static CIwVec2 g_Zero = new CIwVec2(0, 0);

				public CIwVec2(int x, int y)
				{
					this.x = x;
					this.y = y;
				}

				public override int GetHashCode()
				{
					return x.GetHashCode() ^ y.GetHashCode();
				}
				public override bool Equals(object obj)
				{
					if (!(obj is CIwVec2))
						return false;

					return this.Equals((CIwVec2)obj);
				}
				public bool Equals(CIwVec2 other)
				{
					return
						x == other.x &&
						y == other.y;
				}
				public static bool operator ==(CIwVec2 left, CIwVec2 right)
				{
					return left.Equals(right);
				}
				public static bool operator !=(CIwVec2 left, CIwVec2 right)
				{
					return !left.Equals(right);
				}
				public override string ToString()
				{
					return String.Format(CultureInfo.InvariantCulture, "{{{0},{1}}}", x, y);
				}
			}
}

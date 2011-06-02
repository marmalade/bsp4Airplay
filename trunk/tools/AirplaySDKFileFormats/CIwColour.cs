using System;
using System.Globalization;
namespace AirplaySDKFileFormats
{
	public struct CIwColour
	{
		public byte a;
		public byte r;
		public byte g;
		public byte b;

		public CIwColour(byte r,byte g,byte b,byte a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
		public CIwColour(byte r,byte g,byte b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 255;
		}

		public override int GetHashCode()
		{
			return r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode() ^ a.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is CIwColour))
				return false;

			return this.Equals((CIwColour)obj);
		}
		public bool Equals(CIwColour other)
		{
			return
				r == other.r &&
				g == other.g &&
				b == other.b &&
				a == other.a;
		}
		public static bool operator ==(CIwColour left, CIwColour right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(CIwColour left, CIwColour right)
		{
			return !left.Equals(right);
		}
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2},{3}}}", r, g, b, a);
		}

		public static CIwColour White = new CIwColour(255,255,255);
	}
}

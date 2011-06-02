using System;
using System.Globalization;

namespace AirplaySDKFileFormats
{
	public struct CIwQuat
	{
		public float w;
		public float x;
		public float y;
		public float z;

		public CIwQuat(float s, float x, float y, float z)
		{
			this.w = s;
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2},{3}}}", w, x, y, z);
		}
	}
}

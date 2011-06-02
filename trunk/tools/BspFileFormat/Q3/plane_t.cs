using ReaderUtils;

namespace BspFileFormat.Q3
{
	public class plane_t
	{
		public Vector3 normal;               // Vector orthogonal to plane (Nx,Ny,Nz)
		// with Nx2+Ny2+Nz2 = 1
		public float dist;               // Offset to plane, along the normal vector.
		// Distance from (0,0,0) to the plane

		public void Read(System.IO.BinaryReader source)
		{
			normal.X = source.ReadSingle();
			normal.Y = source.ReadSingle();
			normal.Z = source.ReadSingle();
			dist = source.ReadSingle();
		}
	};
}

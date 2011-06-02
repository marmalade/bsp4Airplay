using System;
using System.Globalization;
using System.Collections.Generic;

namespace AirplaySDKFileFormats
{
	public class Cb4aCollisionMeshSoupFaceEdge
	{
		CIwPlane Plane;
	}
	public class Cb4aCollisionMeshSoupEdge
	{
		public CIwVec3 V0;
		public CIwVec3 V1;

		public override int GetHashCode()
		{
			return V0.GetHashCode() ^ V1.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Cb4aCollisionMeshSoupEdge))
				return false;

			return this.Equals((Cb4aCollisionMeshSoupEdge)obj);
		}
		public bool Equals(Cb4aCollisionMeshSoupEdge other)
		{
			return
				(V0 == other.V0 &&
				V1 == other.V1) || (V1 == other.V0 &&
				V0 == other.V1);
		}
		public static bool operator ==(Cb4aCollisionMeshSoupEdge left, Cb4aCollisionMeshSoupEdge right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Cb4aCollisionMeshSoupEdge left, Cb4aCollisionMeshSoupEdge right)
		{
			return !left.Equals(right);
		}
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "({0}-{1})", V0, V1);
		}
	}
	public class Cb4aCollisionMeshSoupFace
	{
		public int startPlane;
		public int numPlanes;
		//public CIwVec3 Normal;
		//public int Distance;
		//public List<Cb4aCollisionMeshSoupFaceEdge> edges = new List<Cb4aCollisionMeshSoupFaceEdge>();
	}
	public class Cb4aCollisionMeshSoup : CIwParseable, Ib4aCollider
	{
		public List<CIwPlane> Planes = new List<CIwPlane>();

		public List<Cb4aCollisionMeshSoupFace> Faces = new List<Cb4aCollisionMeshSoupFace>();
		//public List<CIwVec3> Vertices = new List<CIwVec3>();
		//public List<Cb4aCollisionMeshSoupEdge> Edges = new List<Cb4aCollisionMeshSoupEdge>();
		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_planes", Planes.Count);
			foreach (var v in Planes)
				writer.WriteArray("plane", new int[] { v.v.x, v.v.y, v.v.z,v.k });
			//writer.WriteKeyVal("num_vertices", Vertices.Count);
			//foreach (var v in Vertices)
			//    writer.WriteVec3("v",v);
			//writer.WriteKeyVal("num_edges", Edges.Count);
			//foreach (var e in Edges)
			//{
			//    CIwVec3 d = e.V1 - e.V0;

			//    writer.BeginWriteLine();
			//    writer.Write(string.Format(CultureInfo.InvariantCulture, "e {0}",d.Length));
			//    writer.Write(string.Format(CultureInfo.InvariantCulture, " {{{0},{1},{2}}}", e.V0.x, e.V0.y, e.V0.z));
			//    writer.Write(string.Format(CultureInfo.InvariantCulture, " {{{0},{1},{2}}}", d.x, d.y, d.z));
			//    writer.EndWriteLine();
			//}
			writer.WriteKeyVal("num_faces", Faces.Count);
			foreach (var f in Faces)
			{
				writer.WriteArray("face ", new int[] { f.startPlane,f.numPlanes });
				//writer.WriteLine("next_face");
				//writer.WriteArray("face_p", new int[] { f.Normal.x, f.Normal.y, f.Normal.z, f.Distance });
				//writer.WriteKeyVal("num_face_edges", f.edges.Count);
				//foreach (var e in f.edges)
				//{
				//    writer.WriteArray("edge_p", new int[] { e.Normal.x, e.Normal.y, e.Normal.z, e.Distance });
				//}
			}
		}
	};
	public interface Ib4aCollider
	{
	};
}

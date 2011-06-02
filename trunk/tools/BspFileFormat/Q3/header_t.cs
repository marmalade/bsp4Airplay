using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BspFileFormat.Q3
{
	public class header_t
	{
		public uint magic;      // magic number ("IBSP")
		public uint version;

		public dentry_t entities; //Game-related object descriptions.
		public dentry_t textures; //Surface descriptions.
		public dentry_t planes; //Planes used by map geometry.
		public dentry_t nodes; //BSP tree nodes.
		public dentry_t leafs; //BSP tree leaves.
		public dentry_t leaffaces; //Lists of face indices, one list per leaf.
		public dentry_t leafbrushes; //Lists of brush indices, one list per leaf.
		public dentry_t models; //Descriptions of rigid world geometry in map.
		public dentry_t brushes; //Convex polyhedra used to describe solid space.
		public dentry_t brushsides; //Brush surfaces.
		public dentry_t vertexes; //Vertices used to describe faces.
		public dentry_t meshverts; //Lists of offsets, one list per mesh.
		public dentry_t effects; //List of special map effects.
		public dentry_t faces; //Surface geometry.
		public dentry_t lightmaps; //Packed lightmap data.
		public dentry_t lightvols; //Local illumination data.
		public dentry_t visdata; //Cluster-cluster visibility data.

		public void Read(BinaryReader source)
		{
			magic = source.ReadUInt32();
			version = source.ReadUInt32();
			entities.Read(source);
			textures.Read(source);
			planes.Read(source); //Planes used by map geometry.
			nodes.Read(source); //BSP tree nodes.
			leafs.Read(source); //BSP tree leaves.
			leaffaces.Read(source); //Lists of face indices, one list per leaf.
			leafbrushes.Read(source); //Lists of brush indices, one list per leaf.
			models.Read(source); //Descriptions of rigid world geometry in map.
			brushes.Read(source); //Convex polyhedra used to describe solid space.
			brushsides.Read(source); //Brush surfaces.
			vertexes.Read(source); //Vertices used to describe faces.
			meshverts.Read(source); //Lists of offsets, one list per mesh.
			effects.Read(source); //List of special map effects.
			faces.Read(source); //Surface geometry.
			lightmaps.Read(source); //Packed lightmap data.
			lightvols.Read(source); //Local illumination data.
			visdata.Read(source); //Cluster-cluster visibility data.
		}
	}
}

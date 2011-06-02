using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;

namespace BspFileFormat.Q2
{
	public class header_t
	{
		public uint magic;      // magic number ("IBSP")
		public uint version;

		public dentry_t entities; //MAP entity text buffer	
		public dentry_t planes; //Plane array	
		public dentry_t vertices; //Vertex array	
		public dentry_t visibility; //Compressed PVS data and directory for all clusters	
		public dentry_t nodes; //Internal node array for the BSP tree	
		public dentry_t textureInformation; //Face texture application array	
		public dentry_t faces; //Face array	
		public dentry_t lightmaps; //Lightmaps	
		public dentry_t leaves; //Internal leaf array of the BSP tree	
		public dentry_t leafFaceTable; //Index lookup table for referencing the face array from a leaf	
		public dentry_t leafBrushTable; //?	
		public dentry_t edges; //Edge array	
		public dentry_t faceEdgeTable; //Index lookup table for referencing the edge array from a face	
		public dentry_t models; //?	
		public dentry_t brushes; //?	
		public dentry_t brushSides; //?	
		public dentry_t pop; //?	
		public dentry_t areas; //?       	
		public dentry_t areaPortals; //?

		internal void Read(System.IO.BinaryReader source)
		{
			magic = source.ReadUInt32();
			version = source.ReadUInt32();
			entities.Read(source);
			planes.Read(source); //Plane array	
			vertices.Read(source); //Vertex array	
			visibility.Read(source); //Compressed PVS data and directory for all clusters	
			nodes.Read(source); //Internal node array for the BSP tree	
			textureInformation.Read(source); //Face texture application array	
			faces.Read(source); //Face array	
			lightmaps.Read(source); //Lightmaps	
			leaves.Read(source); //Internal leaf array of the BSP tree	
			leafFaceTable.Read(source); //Index lookup table for referencing the face array from a leaf	
			leafBrushTable.Read(source); //?	
			edges.Read(source); //Edge array	
			faceEdgeTable.Read(source); //Index lookup table for referencing the edge array from a face	
			models.Read(source); //?	
			brushes.Read(source); //?	
			brushSides.Read(source); //?	
			pop.Read(source); //?	
			areas.Read(source); //?       	
			areaPortals.Read(source); //?
		}
	}
}
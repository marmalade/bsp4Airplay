using System;
using System.Collections.Generic;
using System.Text;

namespace BspFileFormat.HL2
{
	public class header_t
	{
		public uint magic;      // magic number ("VBSP")
		public uint version;

		public dentry_t Entities; //Map entities
		public dentry_t Planes; //Plane array
		public dentry_t Texdata; //Index to texture names
		public dentry_t Vertexes; //Vertex array
		public dentry_t Visibility; //Compressed visibility bit arrays
		public dentry_t Nodes; //BSP tree nodes  
		public dentry_t Texinfo; //Face texture array
		public dentry_t Faces; //Face array
		public dentry_t Lighting; //Lightmap samples
		public dentry_t Occlusion; //Occlusion data(?)
		public dentry_t Leafs; //BSP tree leaf nodes
		public dentry_t Unused11; //
		public dentry_t Edges; //Edge array
		public dentry_t Surfedges; //Index of edges
		public dentry_t Models; //Brush models (geometry of brush entities)
		public dentry_t Worldlights; //Light entities
		public dentry_t LeafFaces; //Index to faces in each leaf
		public dentry_t LeafBrushes; //Index to brushes in each leaf
		public dentry_t Brushes; //Brush array
		public dentry_t Brushsides; //Brushside array
		public dentry_t Areas; //Area array
		public dentry_t AreaPortals; //Portals between areas
		public dentry_t Portals; //Polygons defining the boundary between adjacent leaves(?)
		public dentry_t Clusters; //Leaves that are enterable by the player
		public dentry_t PortalVerts; //Vertices of portal polygons
		public dentry_t Clusterportals; //Polygons defining the boundary between adjacent clusters(?)
		public dentry_t Dispinfo; //Displacement surface array
		public dentry_t OriginalFaces; //Brush faces array before BSP splitting
		public dentry_t Unused28; //
		public dentry_t PhysCollide; //Physics collision data(?)
		public dentry_t VertNormals; //Vertex normals(?)
		public dentry_t VertNormalIndices; //Vertex normal index array(?)
		public dentry_t DispLightmapAlphas; //Displacement lightmap data(?)
		public dentry_t DispVerts; //Vertices of displacement surface meshes
		public dentry_t DispLightmapSamplePos; //Displacement lightmap data(?)
		public dentry_t GameLump; //Game-specific data lump
		public dentry_t LeafWaterData; // (?)
		public dentry_t Primitives; //Non-polygonal primatives(?)
		public dentry_t PrimVerts; // (?)
		public dentry_t PrimIndices; //(?)
		public dentry_t Pakfile; //Embedded uncompressed-Zip format file
		public dentry_t ClipPortalVerts; //(?)
		public dentry_t Cubemaps; //Env_cubemap location array
		public dentry_t TexdataStringData; //Texture name data
		public dentry_t TexdataStringTable; //Index array into texdata string data
		public dentry_t Overlays; //Info_overlay array       
		public dentry_t LeafMinDistToWater; //(?)
		public dentry_t FaceMacroTextureInfo; //(?)
		public dentry_t DispTris; //Displacement surface triangles
		public dentry_t PhysCollideSurface; //Physics collision surface data(?)
		public dentry_t Unused50; //
		public dentry_t Unused51; //
		public dentry_t Unused52; //
		public dentry_t LightingHDR; //HDR related lighting data(?)
		public dentry_t WorldlightsHDR; //HDR related worldlight data(?)
		public dentry_t LeaflightHDR1; //HDR related leaf lighting data(?)
		public dentry_t LeaflightHDR2; //HDR related leaf lighting data(?)
		public dentry_t Unused57;
		public dentry_t Unused58;
		public dentry_t Unused59;
		public dentry_t Unused60;
		public dentry_t Unused61;
		public dentry_t Unused62;
		public dentry_t Unused63;

		public uint revision;

		internal void Read(System.IO.BinaryReader source)
		{
			magic = source.ReadUInt32();
			version = source.ReadUInt32();
			bool is21=version>=21;
			Entities.Read(source, is21); //Map entities
			Planes.Read(source, is21); //Plane array
			Texdata.Read(source, is21); //Index to texture names
			Vertexes.Read(source, is21); //Vertex array
			Visibility.Read(source, is21); //Compressed visibility bit arrays
			Nodes.Read(source, is21); //BSP tree nodes  
			Texinfo.Read(source, is21); //Face texture array
			Faces.Read(source, is21); //Face array
			Lighting.Read(source, is21); //Lightmap samples
			Occlusion.Read(source, is21); //Occlusion data(?)
			Leafs.Read(source, is21); //BSP tree leaf nodes
			Unused11.Read(source, is21); //
			Edges.Read(source, is21); //Edge array
			Surfedges.Read(source, is21); //Index of edges
			Models.Read(source, is21); //Brush models (geometry of brush entities)
			Worldlights.Read(source, is21); //Light entities
			LeafFaces.Read(source, is21); //Index to faces in each leaf
			LeafBrushes.Read(source, is21); //Index to brushes in each leaf
			Brushes.Read(source, is21); //Brush array
			Brushsides.Read(source, is21); //Brushside array
			Areas.Read(source, is21); //Area array
			AreaPortals.Read(source, is21); //Portals between areas
			Portals.Read(source, is21); //Polygons defining the boundary between adjacent leaves(?)
			Clusters.Read(source, is21); //Leaves that are enterable by the player
			PortalVerts.Read(source, is21); //Vertices of portal polygons
			Clusterportals.Read(source, is21); //Polygons defining the boundary between adjacent clusters(?)
			Dispinfo.Read(source, is21); //Displacement surface array
			OriginalFaces.Read(source, is21); //Brush faces array before BSP splitting
			Unused28.Read(source, is21); //
			PhysCollide.Read(source, is21); //Physics collision data(?)
			VertNormals.Read(source, is21); //Vertex normals(?)
			VertNormalIndices.Read(source, is21); //Vertex normal index array(?)
			DispLightmapAlphas.Read(source, is21); //Displacement lightmap data(?)
			DispVerts.Read(source, is21); //Vertices of displacement surface meshes
			DispLightmapSamplePos.Read(source, is21); //Displacement lightmap data(?)
			GameLump.Read(source, is21); //Game-specific data lump
			LeafWaterData.Read(source, is21); // (?)
			Primitives.Read(source, is21); //Non-polygonal primatives(?)
			PrimVerts.Read(source, is21); // (?)
			PrimIndices.Read(source, is21); //(?)
			Pakfile.Read(source, is21); //Embedded uncompressed-Zip format file
			ClipPortalVerts.Read(source, is21); //(?)
			Cubemaps.Read(source, is21); //Env_cubemap location array
			TexdataStringData.Read(source, is21); //Texture name data
			TexdataStringTable.Read(source, is21); //Index array into texdata string data
			Overlays.Read(source, is21); //Info_overlay array       
			LeafMinDistToWater.Read(source, is21); //(?)
			FaceMacroTextureInfo.Read(source, is21); //(?)
			DispTris.Read(source, is21); //Displacement surface triangles
			PhysCollideSurface.Read(source, is21); //Physics collision surface data(?)
			Unused50.Read(source, is21); //
			Unused51.Read(source, is21); //
			Unused52.Read(source, is21); //
			LightingHDR.Read(source, is21); //HDR related lighting data(?)
			WorldlightsHDR.Read(source, is21); //HDR related worldlight data(?)
			LeaflightHDR1.Read(source, is21); //HDR related leaf lighting data(?)
			LeaflightHDR2.Read(source, is21); //HDR related leaf lighting data(?)
			Unused57.Read(source, is21);
			Unused58.Read(source, is21);
			Unused59.Read(source, is21);
			Unused60.Read(source, is21);
			Unused61.Read(source, is21);
			Unused62.Read(source, is21);
			Unused63.Read(source, is21);

			revision = source.ReadUInt32();
		}
	}
}

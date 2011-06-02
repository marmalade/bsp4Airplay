using System;
using System.Collections.Generic;
using System.Text;
using ReaderUtils;
using System.IO;
using BspFileFormat.Utils;
using System.Drawing;
using System.Globalization;

namespace BspFileFormat.Q1HL1
{
	public class QuakeReader
	{
		protected long startOfTheFile;
		protected header_t header;
		protected List<Vector3> vertices;
		protected List<edge_t> edges;
		protected List<BspEmbeddedTexture> textures;
		protected List<face_t> faces;
		protected List<model_t> models;
		protected List<plane_t> planes;
		protected List<node_t> nodes;
		protected List<dleaf_t> dleaves;
		protected List<surface_t> surfaces;
		protected List<BspTreeLeaf> leaves = new List<BspTreeLeaf>();

		protected byte[] visilist;
		protected byte[] lightmap;
		protected ushort[] listOfFaces;
		protected int[] listOfEdges;
		protected string entities;
		private int maxUsedEdge;

		public void ReadBsp(System.IO.BinaryReader source, BspDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;

			ReadHeader(source);
			ReadEntities(source);
			ReadPlanes(source);
			ReadTextures(source);
			ReadTextureInfos(source);
			ReadVisilist(source);
			ReadEdges(source);
			ReadNodes(source);
			ReadLeaves(source);
			ReadVertices(source);
			ReadLightmap(source);
			ReadFaces(source);
			ReadListOfFaces(source);
			ReadListOfEdges(source);
			ReadModels(source);
			
			//if (textures != null)
			//    foreach (var tex in textures)
			//        dest.AddTexture(tex);

			//if (models != null)
			//{
			//    //foreach (var model in models)
			//    for (int i = 1; i < models.Count; ++i)
			//        dest.AddModel(BuildGeometry(models[i], i));
			//    //BuildGeometry(models[0], 0);
			//}

			BuildLeaves();
			if (nodes != null && nodes.Count > 0)
			{
				dest.Tree = BuildNode(nodes[0]);
			}
			BuildVisibilityList();
			BuildClusters();


			ReaderHelper.BuildEntities(entities, dest);
		}

		private void BuildClusters()
		{
			FaceToLeafMap faceMap = new FaceToLeafMap(faces.Count);
			for (int i=0; i<dleaves.Count; ++i)
			{
				var dleaf = dleaves[i];
				BuilFaceToLeafMap(faceMap, i, dleaf);
				foreach (var vis in dleaf.VisibleLeaves)
				{
					dleaf = dleaves[vis];
					BuilFaceToLeafMap(faceMap, i, dleaf);
				}
			}
			var keys = faceMap.FindUniqueKeys();

			foreach (var k in keys)
			{
				if (k.Faces.Count > 0)
				{
					var geo = BuildGeometry(k.Faces);
					if (geo != null)
					{
						foreach (var l in k.Key.Leaves)
							leaves[l.Key].Geometries.Add(geo);
					}
				}
			}
		}

		

		private void BuilFaceToLeafMap(FaceToLeafMap faceMap, int i, dleaf_t dleaf)
		{
			if (dleaf.lface_num >= 0)
				for (int j = dleaf.lface_id; j < dleaf.lface_id + dleaf.lface_num; ++j)
					faceMap.Faces[listOfFaces[j]].AddLeaf(i);
		}

		

		

		private void ReadEntities(BinaryReader source)
		{
			SeekDir(source, header.entities);
			int size = (int)(header.entities.size);
			entities = Encoding.ASCII.GetString(source.ReadBytes(size));
		}

		private BspCollisionFaceSoup BuildFaceSoup(ushort fromFace, ushort numFaces)
		{
			if (numFaces == 0)
				return null;
			var soup = new BspCollisionFaceSoup();
			//var vertexMap = new Dictionary<Vector3, int>();
			//var edgeMap = new Dictionary<BspCollisionFaceSoupEdge, int>();

			for (uint i = fromFace; i < fromFace + numFaces; ++i)
			{
				ushort faceIndex = listOfFaces[i];
				var face = faces[faceIndex];
				var texName = textures[(int)surfaces[face.texinfo_id].texture_id].Name;
				if (texName == "aaatrigger")
					continue;

				var soupFace = new BspCollisionFaceSoupFace();
				soup.Faces.Add(soupFace);

				//soupFace.Normal = planes[face.plane_id].normal;
				//soupFace.Distance = planes[face.plane_id].dist;
				//if (face.side != 0)
				//{
				//	soupFace.Normal = -soupFace.Normal;
				//	soupFace.Distance = -soupFace.Distance;
				//}
				//float prevDist = float.MinValue;
				//var prevN = Vector3.Zero;
				for (int j = 0; j < (int)face.ledge_num; ++j)
				{
					var listOfEdgesIndex = (int)face.ledge_id + j;
					var edgeIndex = listOfEdges[listOfEdgesIndex];
					Vector3 v0, v1;
					if (edgeIndex >= 0)
					{
						v0 = vertices[edges[edgeIndex].vertex0];
						v1 = vertices[edges[edgeIndex].vertex1];
					}
					else
					{
						v1 = vertices[edges[-edgeIndex].vertex0];
						v0 = vertices[edges[-edgeIndex].vertex1];
					}
					soupFace.Vertices.Add(v0);
					//if (!vertexMap.ContainsKey(v0))
					//{
					//    vertexMap[v0] = soup.Vertices.Count;
					//    soup.Vertices.Add(v0);
					//}
					//if (!vertexMap.ContainsKey(v1))
					//{
					//    vertexMap[v1] = soup.Vertices.Count;
					//    soup.Vertices.Add(v1);
					//}
					//var edgeNormal = Vector3.Cross(v1 - v0, soupFace.Normal);
					//edgeNormal.Normalize();
					//float edgeDist = Vector3.Dot(edgeNormal, v0);
					//if (edgeNormal != prevN || edgeDist != prevDist)
					//{
					//    soupFace.Edges.Add(new BspCollisionFaceSoupFaceEdge() { Normal = edgeNormal, Distance = edgeDist });
					//    prevN = edgeNormal; prevDist = edgeDist;
					//}
				}
			}
			return soup;
		}

		Dictionary<int, BspTexture> faceLightmapObjects = new Dictionary<int, BspTexture>();
		private BspGeometry BuildGeometry(List<int> list)
		{
			if (list == null || list.Count == 0)
				return null;
			var res = new BspGeometry() { Faces = new List<BspGeometryFace>() };

			foreach (var faceIndex in list)
			{
				var face = faces[faceIndex];

				if (face.ledge_num == 0)
					continue;

				plane_t plane = planes[face.plane_id];
				var surf = surfaces[face.texinfo_id];
				int texture_id = (int)surf.texture_id;
				if (textures[texture_id].Name == "aaatrigger")
					continue;
				var faceVertices = new BspGeometryVertex[face.ledge_num];

				Vector2 minUV0 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 minUV1 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 maxUV1 = new Vector2(float.MinValue, float.MinValue);
				for (int j = 0; j < (int)face.ledge_num; ++j)
				{

					var listOfEdgesIndex = (int)face.ledge_id + j;
					if (listOfEdgesIndex >= listOfEdges.Length)
						throw new ApplicationException(string.Format("Edge list index {0} is out of range [0..{1}]", listOfEdgesIndex, listOfEdges.Length - 1));
					var edgeIndex = listOfEdges[listOfEdgesIndex];
					if (edgeIndex >= edges.Count)
						throw new ApplicationException(string.Format("Edge index {0} is out of range [0..{1}]", edgeIndex, edges.Count - 1));
					edge_t edge;
					if (edgeIndex >= 0)
					{
						edge = edges[edgeIndex];
					}
					else
					{
						var flippedEdge = edges[-edgeIndex];
						edge = new edge_t() { vertex0 = flippedEdge.vertex1, vertex1 = flippedEdge.vertex0 };
					}
					var edgesvertex0 = edge.vertex0;
					if (edgesvertex0 >= vertices.Count)
						throw new ApplicationException(string.Format("Vertex index {0} is out of range [0..{1}]", edgesvertex0, vertices.Count - 1));
					var edgesvertex1 = edge.vertex1;
					if (edgesvertex1 >= vertices.Count)
						throw new ApplicationException(string.Format("Vertex index {0} is out of range [0..{1}]", edgesvertex1, vertices.Count - 1));
					BspGeometryVertex vertex = BuildVertex(vertices[(short)edgesvertex0], (face.side == 0) ? plane.normal : -plane.normal, ref surf);
					faceVertices[j] = vertex;
					if (minUV0.X > vertex.UV0.X)
						minUV0.X = vertex.UV0.X;
					if (minUV0.Y > vertex.UV0.Y)
						minUV0.Y = vertex.UV0.Y;
					if (minUV1.X > vertex.UV1.X)
						minUV1.X = vertex.UV1.X;
					if (minUV1.Y > vertex.UV1.Y)
						minUV1.Y = vertex.UV1.Y;
					if (maxUV1.X < vertex.UV1.X)
						maxUV1.X = vertex.UV1.X;
					if (maxUV1.Y < vertex.UV1.Y)
						maxUV1.Y = vertex.UV1.Y;

				}
				minUV0.X = (float)System.Math.Floor(minUV0.X);
				minUV0.Y = (float)System.Math.Floor(minUV0.Y);

				minUV1.X = (float)System.Math.Floor(minUV1.X);
				minUV1.Y = (float)System.Math.Floor(minUV1.Y);
				maxUV1.X = (float)System.Math.Ceiling(maxUV1.X);
				maxUV1.Y = (float)System.Math.Ceiling(maxUV1.Y);
				var sizeLightmap = maxUV1 - minUV1 + new Vector2(1, 1);
				float safeOffset = 0.5f;//0.5f;
				float safeBorderWidth = 1;
				for (int j = 0; j < (int)face.ledge_num; ++j)
				{
					faceVertices[j].UV0 = faceVertices[j].UV0 - minUV0;
					faceVertices[j].UV1.X = (faceVertices[j].UV1.X - minUV1.X + safeOffset) / (sizeLightmap.X + safeBorderWidth);
					faceVertices[j].UV1.Y = (faceVertices[j].UV1.Y - minUV1.Y + safeOffset) / (sizeLightmap.Y + safeBorderWidth);
				}
				if (textures[texture_id].Name == "sky")
				{
					for (int j = 0; j < (int)face.ledge_num; ++j)
						faceVertices[j].UV0 = new Vector2(0, 0);
				}
				BspTexture lightMap = null;
				if (face.lightmap != -1)
				{
					if (!faceLightmapObjects.TryGetValue(face.lightmap, out lightMap))
					{
						var size2 = (sizeLightmap.X) * (sizeLightmap.Y);
						Bitmap faceLightmap = BuildFaceLightmap(face.lightmap, (int)sizeLightmap.X, (int)sizeLightmap.Y);
						faceLightmap = ReaderHelper.BuildSafeLightmap(faceLightmap);
						lightMap = new BspEmbeddedTexture()
						{
							Name = "facelightmap" + face.lightmap,
							mipMaps = new Bitmap[] { faceLightmap },
							Width = faceLightmap.Width,
							Height = faceLightmap.Height
						};
						faceLightmapObjects[face.lightmap] = lightMap;
					}
				}

				var vert0 = faceVertices[0];
				for (int j = 1; j < faceVertices.Length - 1; ++j)
				{
					BspGeometryVertex vert1 = faceVertices[j];
					BspGeometryVertex vert2 = faceVertices[j + 1];
					var geoFace = new BspGeometryFace() { Vertex0 = vert0, Vertex1 = vert1, Vertex2 = vert2, Texture = textures[texture_id], Lightmap = lightMap };
					res.Faces.Add(geoFace);
				}
			}
			return res;
		}
		private BspGeometry BuildGeometry(uint fromFace, uint numFaces)
		{
			List<int> res = new List<int>();
			for (uint i = fromFace; i < fromFace + numFaces; ++i)
			{
				res.Add(listOfFaces[i]);
			}
			return BuildGeometry(res);
		}
		private BspGeometry BuildGeometry(model_t model)
		{
			return BuildGeometry(model.face_id, model.face_num);
			
		}
		//Dictionary<int, Bitmap> faceLightmaps = new Dictionary<int, Bitmap>();
		public virtual Bitmap BuildFaceLightmap(int p, int w, int h)
		{

			Bitmap b;
			//if (faceLightmaps.TryGetValue(p, out b))
			//    return b;
			b = new Bitmap(w, h);
			for (int y=0; y<h;++y)
				for (int x = 0; x < w; ++x)
				{
					b.SetPixel(x, y, Color.FromArgb(lightmap[p], lightmap[p], lightmap[p]));
					++p;
				}
			//faceLightmaps[p] = b;
			return b;
		}

		private BspGeometryVertex BuildVertex(Vector3 vector3, Vector3 n, ref surface_t surf)
		{
			var res = new BspGeometryVertex();
			res.Position = vector3;
			res.Normal = n;
			res.UV0 = new Vector2(Vector3.Dot(surf.vectorS, vector3) + surf.distS, Vector3.Dot(surf.vectorT, vector3) + surf.distT);
			res.UV1 = new Vector2(res.UV0.X / 16.0f, res.UV0.Y/16.0f);
			
			res.UV0 = new Vector2((res.UV0.X) / (float)textures[(int)surf.texture_id].Width, (res.UV0.Y) / (float)textures[(int)surf.texture_id].Height);
			return res;
		}
		private void ReadListOfEdges(BinaryReader source)
		{
			listOfEdges = ReaderHelper.ReadInt32Array(source, header.ledges.size, header.ledges.offset);
			//SeekDir(source, header.ledges);
			//int size = (int)(header.ledges.size / 4);
			//listOfEdges = new short[size];
			//maxUsedEdge = 0;
			//for (int i = 0; i < size; ++i)
			//{
			//    listOfEdges[i] = (short)source.ReadInt32();
			//    if (listOfEdges[i] > maxUsedEdge)
			//        maxUsedEdge = listOfEdges[i];
			//}
		}
		private void ReadListOfFaces(BinaryReader source)
		{
			listOfFaces = ReaderHelper.ReadUInt16Array(source, header.lface.size, header.lface.offset);
		}
		private void BuildLeaves()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				leaves.Add(BuildLeaf(dleaves[i]));
			}
		}
		private void BuildVisibilityList()
		{
			for (int i = 0; i < dleaves.Count; ++i)
			{
				BuildVisibilityList(leaves[i], dleaves[i], dleaves[i].vislist);
			}
		}

		private void BuildVisibilityList(BspTreeLeaf bspTreeLeaf, dleaf_t leaf, int v)
		{
			if (v < 0)
				return;
			// Suppose Leaf is the leaf the player is in.
			for (int L = 1; L < dleaves.Count && v<visilist.Length; v++)
			{
				if (visilist[v] == 0)           // value 0, leaves invisible
				{
					L += 8 * visilist[v + 1];    // skip some leaves
					v++;
				}
				else                          // tag 8 leaves, if needed
				{                           // examine bits right to left
					for (byte bit = 1; bit != 0 && L < dleaves.Count; bit = (byte)(bit << 1), ++L)
					{
						if (0 != (visilist[v] & bit))
						{
							if (L >= leaves.Count)
								throw new ApplicationException(string.Format("leaf index {0} is out of {1}",L,leaves.Count));
							bspTreeLeaf.VisibleLeaves.Add(leaves[L]);
							leaf.VisibleLeaves.Add(L);
						}
					}
				}
			}
		}

		private BspTreeNode BuildNode(node_t node)
		{
			var res = new BspTreeNode();
			res.Mins = new Vector3(node.box.mins[0], node.box.mins[1], node.box.mins[2]);
			res.Maxs = new Vector3(node.box.maxs[0], node.box.maxs[1], node.box.maxs[2]);

			res.PlaneDistance = planes[node.planenum].dist;
			res.PlaneNormal = planes[node.planenum].normal;

			if (0 != (node.front & 0x8000))
				res.Front = leaves[(ushort)~node.front];
			else
				res.Front = BuildNode(nodes[node.front]);
			if (0 != (node.back & 0x8000))
				res.Back = leaves[(ushort)~node.back];
			else
				res.Back = BuildNode(nodes[node.back]);
			return res;
		}

		private BspTreeLeaf BuildLeaf(dleaf_t dleaf)
		{
			var res = new BspTreeLeaf();
			res.Mins = new Vector3(dleaf.box.mins[0], dleaf.box.mins[1], dleaf.box.mins[2]);
			res.Maxs = new Vector3(dleaf.box.maxs[0], dleaf.box.maxs[1], dleaf.box.maxs[2]);
			if (dleaf.lface_num > 0)
				res.Colliders.Add(BuildFaceSoup(dleaf.lface_id, dleaf.lface_num));
			return res;
		}


		protected virtual void ReadTextures(BinaryReader source)
		{
			if (header.miptex.size == 0)
				return;
			SeekDir(source, header.miptex);
			mipheader_t hdr = new mipheader_t(); 
			hdr.Read(source);
			textures = new List<BspEmbeddedTexture>(hdr.offset.Length);
			
			foreach (var offset in hdr.offset)
			{
				source.BaseStream.Seek(startOfTheFile + header.miptex.offset + offset, SeekOrigin.Begin);
				miptex_t miptex = new miptex_t();
				var texPos = source.BaseStream.Position;
				miptex.Read(source);

				var tex = new BspEmbeddedTexture() { 
					Name = miptex.name, Width = (int)miptex.width, Height = (int)miptex.height, 
					Sky = miptex.name == "sky", 
					Transparent = (miptex.name[0] == '{' || miptex.name[0] == '#') };
				if (miptex.offset1 > 0)
				{
					tex.mipMaps = new Bitmap[4];
					int w = (int)miptex.width;
					int h = (int)miptex.height;
					source.BaseStream.Seek(texPos + miptex.offset1, SeekOrigin.Begin);
					var palette = GetPalette(source, miptex);
					tex.mipMaps[0] = ReadBitmapData(source, miptex.alphaTest, w,h,palette);
					w /= 2; h /= 2;
					source.BaseStream.Seek(texPos + miptex.offset2, SeekOrigin.Begin);
					tex.mipMaps[1] = ReadBitmapData(source, miptex.alphaTest, w, h, palette);
					w /= 2; h /= 2;
					source.BaseStream.Seek(texPos + miptex.offset4, SeekOrigin.Begin);
					tex.mipMaps[2] = ReadBitmapData(source, miptex.alphaTest, w, h, palette);
					w /= 2; h /= 2;
					source.BaseStream.Seek(texPos + miptex.offset8, SeekOrigin.Begin);
					tex.mipMaps[3] = ReadBitmapData(source, miptex.alphaTest, w, h, palette);
				}
				textures.Add(tex);
			}
		}

		public virtual Color[] GetPalette(BinaryReader source, miptex_t tex)
		{
			return q1palette.palette;
		}

		public virtual Bitmap ReadBitmapData(BinaryReader source, bool a, int w, int h, Color[] palette)
		{
			var bmp = new Bitmap(w, h);
			for (int y = 0; y < h; ++y)
				for (int x = 0; x < w; ++x)
				{
					var index = (int)source.ReadByte();
					if (a && index == 255)
						bmp.SetPixel(x, y, Color.FromArgb(0,0,0,0));
					else
						bmp.SetPixel(x, y, palette[index]);
				}
			return bmp;
		}
		private void ReadTextureInfos(BinaryReader source)
		{
			surfaces = ReaderHelper.ReadStructs<surface_t>(source, header.texinfo.size, header.texinfo.offset + startOfTheFile, 40);
		}

		private void ReadNodes(BinaryReader source)
		{
			nodes = ReaderHelper.ReadStructs<node_t>(source, header.nodes.size, header.nodes.offset + startOfTheFile, 24);
			
		}

		private void ReadLeaves(BinaryReader source)
		{
			dleaves = ReaderHelper.ReadStructs<dleaf_t>(source, header.leaves.size, header.leaves.offset + startOfTheFile, 28);
			
		}

		private void ReadPlanes(BinaryReader source)
		{
			planes = ReaderHelper.ReadStructs<plane_t>(source, header.planes.size, header.planes.offset + startOfTheFile, 20);
		}
		private void ReadVisilist(BinaryReader source)
		{
			SeekDir(source, header.visilist);
			int size = (int)(header.visilist.size);
			visilist = source.ReadBytes(size);
		}

		public virtual void ReadLightmap(BinaryReader source)
		{
			SeekDir(source, header.lightmaps);
			int size = (int)(header.lightmaps.size);
			lightmap = source.ReadBytes(size);
		}

		private void ReadFaces(BinaryReader source)
		{
			faces = ReaderHelper.ReadStructs<face_t>(source, header.faces.size, header.faces.offset + startOfTheFile, 20);
		}

		private void ReadEdges(BinaryReader source)
		{
			edges = ReaderHelper.ReadStructs<edge_t>(source, header.edges.size, header.edges.offset + startOfTheFile, 4);
		}

		private void ReadModels(BinaryReader source)
		{
			models = ReaderHelper.ReadStructs<model_t>(source, header.models.size, header.models.offset + startOfTheFile, 64);
			
		}

		private void ReadVertices(BinaryReader source)
		{
			SeekDir(source, header.vertices);
			if (header.vertices.size % 12 != 0)
				throw new Exception();
			int size = (int)(header.vertices.size / (12));
			vertices = new List<Vector3>(size);
			for (int i = 0; i < size; ++i)
			{
				var v = new Vector3();
				v.X = source.ReadSingle();
				v.Y = source.ReadSingle();
				v.Z = source.ReadSingle();
				vertices.Add(v);
			}
			if (source.BaseStream.Position + startOfTheFile != header.vertices.size + header.vertices.offset)
				throw new Exception();
		}

		private void SeekDir(BinaryReader source, dentry_t dir)
		{
			source.BaseStream.Seek(startOfTheFile + dir.offset, SeekOrigin.Begin);
		}

		private void ReadHeader(BinaryReader source)
		{
			header = new header_t();
			header.Read(source);
		}
	}
}

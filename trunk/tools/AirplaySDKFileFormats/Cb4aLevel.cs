using System;
using System.Globalization;
using System.Collections.Generic;
using AirplaySDKFileFormats.Model;

namespace AirplaySDKFileFormats
{
	public class Cb4aLevel: CIwResource
	{
		public List<Cb4aNode> Nodes = new List<Cb4aNode>();
		public List<Cb4aLeaf> Leaves = new List<Cb4aLeaf>();
		public List<Cb4aEntity> Entities = new List<Cb4aEntity>();
		public List<CIwPlane> Planes = new List<CIwPlane>();
		public List<Cb4aLevelVertexBuffer> VertexBuffers = new List<Cb4aLevelVertexBuffer>();
		//public List<Cb4aLevelVBCluster> clusters = new List<Cb4aLevelVBCluster>();
		public List<Cb4aLevelVBSubcluster> subclusters = new List<Cb4aLevelVBSubcluster>();
		public List<Cb4aLevelMaterial> Materials = new List<Cb4aLevelMaterial>();

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			writer.WriteKeyVal("num_materials", Materials.Count);
			foreach (var l in Materials)
			{
				l.WrtieToStream(writer);
			}
			if (Planes.Count > 0)
			{
				writer.WriteKeyVal("num_planes", Planes.Count);
				foreach (var l in Planes)
				{
					writer.WriteArray("plane", new int[] { (int)l.v.x, (int)l.v.y, (int)l.v.z, (int)(l.k * AirplaySDKMath.IW_GEOM_ONE) });
				}
			}
			writer.WriteKeyVal("num_vbs", VertexBuffers.Count);
			foreach (var l in VertexBuffers)
			{
				l.WrtieToStream(writer);
			}
			writer.WriteKeyVal("num_clusters", subclusters.Count);
			foreach (var l in subclusters)
			{
				l.WrtieToStream(writer);
			}
			writer.WriteKeyVal("num_leaves", Leaves.Count);
			foreach (var l in Leaves)
			{
				l.WrtieToStream(writer);
			}
			writer.WriteKeyVal("num_nodes", Nodes.Count);
			foreach (var l in Nodes)
			{
				l.WrtieToStream(writer);
			}
			writer.WriteKeyVal("num_entities", Entities.Count);
			foreach (var l in Entities)
			{
				l.WrtieToStream(writer);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BspFileFormat
{
	public class BspTreeLeaf : BspTreeElement
	{
		public List<BspTreeLeaf> VisibleLeaves = new List<BspTreeLeaf>();
		public List<BspCollisionObject> Colliders = new List<BspCollisionObject>();
		public List<BspGeometry> Geometries = new List<BspGeometry>();
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ReaderUtils;

namespace BspFileFormat
{
	public class BspTreeNode : BspTreeElement
	{
		public Vector3 PlaneNormal;
		public float PlaneDistance;

		public BspTreeElement Front { get; set; }
		public BspTreeElement Back { get; set; }
	}
}

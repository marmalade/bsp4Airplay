using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Atlasing
{
	public class AtlasItem
	{
		Bitmap bitmap;
		Atlas atlas;
		public AtlasItem(Atlas atlas, Bitmap bitmap)
		{
			this.atlas = atlas;
			this.bitmap = bitmap;
		}

		public Bitmap Bitmap
		{
			get
			{
				return bitmap;
			}
		}
		public Atlas Atlas
		{
			get
			{
				return atlas;
			}
		}
		Point position;
		public Point Position
		{
			get
			{
				atlas.BuildAtlas();
				return position;
			}
			set
			{
				position = value;
			}
		}
		public Size Size
		{
			get
			{
				return bitmap.Size;
			}
		}
	}
}

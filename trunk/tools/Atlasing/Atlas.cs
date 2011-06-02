using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;

namespace Atlasing
{
	public class AtlasOcupiedArea
	{
		public Point Point;
		public Size Size;
	}
	/// <summary>
	/// Sort atlas items with bigger area first
	/// </summary>
	public class AtlasItemComparer : Comparer<AtlasItem>
	{
		public override int Compare(AtlasItem x, AtlasItem y)
		{
			return y.Size.Width * y.Size.Height-x.Size.Width * x.Size.Height;
		}
	}
	public class Atlas
	{
		Dictionary<Bitmap, AtlasItem> items = new Dictionary<Bitmap, AtlasItem>();
		SortedList<int, Point> recomendedCorners = new SortedList<int, Point>();
		//List<AtlasOcupiedArea> OcupiedAreas = new List<AtlasOcupiedArea>();
		List<bool[]> occupiedAreas = new List<bool[]>();
		bool atlasIsValid = false;
		public Atlas()
		{
		}

		public AtlasItem Add(Bitmap bitmap)
		{
			AtlasItem i;
			if (!items.TryGetValue(bitmap, out i))
			{
				i = new AtlasItem(this, bitmap);
				items[bitmap] = i;
				InvalidateAtlas();
			}
			return i;
		}
		Bitmap bitmap;
		public Bitmap Bitmap
		{
			get
			{
				BuildAtlas();
				RenderAtlas();
				return bitmap;
			}
		}
		int bitmapWidth = 1;
		int bitmapHeight = 0;
		private void RenderAtlas()
		{
			if (bitmap != null)
				return;
			bitmap = new Bitmap(bitmapWidth, bitmapHeight);
			using (Graphics gr = Graphics.FromImage(bitmap))
			{
				gr.Clear(Color.Black);
				foreach (var i in items)
				{
					gr.DrawImage(i.Value.Bitmap, i.Value.Position);
				}
			}
		}
        private void InvalidateAtlas()
		{
			atlasIsValid = false;
			bitmap = null;
			occupiedAreas.Clear();
			recomendedCorners.Clear();
		}
		public void BuildAtlas()
		{
			if (atlasIsValid)
				return;
			atlasIsValid = true;

			AtlasItem[] itemsArray = new AtlasItem[items.Count];
            items.Values.CopyTo(itemsArray,0);

			Array.Sort(itemsArray, new AtlasItemComparer());

			int area = 0;
			foreach (var i in itemsArray)
				area += i.Size.Width * i.Size.Height;
			var areaSize = Math.Sqrt(area);
			bitmapWidth = 1;
			bitmapHeight = 0;
			while ((double)bitmapWidth < areaSize)
				bitmapWidth = bitmapWidth << 1;

			recomendedCorners.Add(0, new Point(0,0));
			foreach (var i in itemsArray)
			{
				if (!TryToPlaceItem(i))
					throw new ApplicationException();
			}
			var maxH = 1;
			foreach (var i in itemsArray)
			{
				if (i.Position.Y + i.Size.Height > maxH)
					maxH = i.Position.Y + i.Size.Height;
			}
			bitmapHeight = 1;
			while (bitmapHeight < maxH)
				bitmapHeight = bitmapHeight << 1;
			
		}

		private bool TryToPlaceItem(AtlasItem i)
		{
			var size = i.Size;
			var e = recomendedCorners.GetEnumerator();
			for (int cornerIndex = 0; cornerIndex<recomendedCorners.Count; ++cornerIndex)
			{
				e.MoveNext();
				var c = e.Current;
				var point = c.Value;
				if (IsAreaAvailable(point, size))
				{
					i.Position = point;
					Occupie(point, size);
					
					recomendedCorners.RemoveAt(cornerIndex);
					var p = new Point(point.X + size.Width, point.Y);
					var key = p.Y * bitmapWidth + p.X;
					if (!recomendedCorners.ContainsKey(key) && IsAreaAvailable(p, new Size(1, 1)))
					{
						recomendedCorners.Add(key, p);
					}
					p = new Point(point.X, point.Y + size.Height);
					key = p.Y * bitmapWidth + p.X;
					if (!recomendedCorners.ContainsKey(key) && IsAreaAvailable(p, new Size(1, 1)))
					{
						recomendedCorners.Add(key, p);
					}
					return true;
				}
			}
			return false;
		}

		private void Occupie(Point c, Size size)
		{
			//OcupiedAreas.Add(new AtlasOcupiedArea() { Point = point, Size = size });
			for (int y = c.Y; y < c.Y + size.Height; ++y)
			{
				while (y >= occupiedAreas.Count)
					occupiedAreas.Add(new bool[bitmapWidth]);
				for (int x = c.X; x < c.X + size.Width; ++x)
					occupiedAreas[y][x] = true;
			}
		}

		private bool IsAreaAvailable(Point c, Size size)
		{
			if (c.X + size.Width > bitmapWidth)
				return false;
			if (bitmapHeight > 0 && c.Y + size.Height > bitmapHeight)
				return false;
			for (int y=c.Y; y<c.Y+size.Height; ++y)
			{
				if (y >= occupiedAreas.Count)
					return true;
				for (int x = c.X; x < c.X + size.Width; ++x)
					if (occupiedAreas[y][x])
						return false;
			}
				
			//foreach (var a in OcupiedAreas)
			//{
			//    if (a.Point.X >= c.X + size.Width)
			//        continue;
			//    if (a.Point.Y >= c.Y + size.Height)
			//        continue;
			//    if (a.Point.X+a.Size.Width <= c.X)
			//        continue;
			//    if (a.Point.Y + a.Size.Height <= c.Y)
			//        continue;
			//    return false;
			//}
			return true;
		}

		public AtlasItem GetItem(Bitmap bmp)
		{
			return items[bmp];
		}
	}
}

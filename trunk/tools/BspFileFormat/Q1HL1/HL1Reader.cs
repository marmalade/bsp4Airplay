using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using System.Drawing;
using System.IO;

namespace BspFileFormat.Q1HL1
{
	public class HL1Reader : QuakeReader, IBspReader
	{
		public HL1Reader()
		{
			//this.palette = new Color[] { Color.FromArgb(0, 0, 0), Color.FromArgb(15, 15, 15), Color.FromArgb(31, 31, 31), Color.FromArgb(47, 47, 47), Color.FromArgb(63, 63, 63), Color.FromArgb(75, 75, 75), Color.FromArgb(91, 91, 91), Color.FromArgb(107, 107, 107), Color.FromArgb(123, 123, 123), Color.FromArgb(139, 139, 139), Color.FromArgb(155, 155, 155), Color.FromArgb(171, 171, 171), Color.FromArgb(187, 187, 187), Color.FromArgb(203, 203, 203), Color.FromArgb(219, 219, 219), Color.FromArgb(235, 235, 235), Color.FromArgb(15, 11, 7), Color.FromArgb(23, 15, 11), Color.FromArgb(31, 23, 11), Color.FromArgb(39, 27, 15), Color.FromArgb(47, 35, 19), Color.FromArgb(55, 43, 23), Color.FromArgb(63, 47, 23), Color.FromArgb(75, 55, 27), Color.FromArgb(83, 59, 27), Color.FromArgb(91, 67, 31), Color.FromArgb(99, 75, 31), Color.FromArgb(107, 83, 31), Color.FromArgb(115, 87, 31), Color.FromArgb(123, 95, 35), Color.FromArgb(131, 103, 35), Color.FromArgb(143, 111, 35), Color.FromArgb(11, 11, 15), Color.FromArgb(19, 19, 27), Color.FromArgb(27, 27, 39), Color.FromArgb(39, 39, 51), Color.FromArgb(47, 47, 63), Color.FromArgb(55, 55, 75), Color.FromArgb(63, 63, 87), Color.FromArgb(71, 71, 103), Color.FromArgb(79, 79, 115), Color.FromArgb(91, 91, 127), Color.FromArgb(99, 99, 139), Color.FromArgb(107, 107, 151), Color.FromArgb(115, 115, 163), Color.FromArgb(123, 123, 175), Color.FromArgb(131, 131, 187), Color.FromArgb(139, 139, 203), Color.FromArgb(0, 0, 0), Color.FromArgb(7, 7, 0), Color.FromArgb(11, 11, 0), Color.FromArgb(19, 19, 0), Color.FromArgb(27, 27, 0), Color.FromArgb(35, 35, 0), Color.FromArgb(43, 43, 7), Color.FromArgb(47, 47, 7), Color.FromArgb(55, 55, 7), Color.FromArgb(63, 63, 7), Color.FromArgb(71, 71, 7), Color.FromArgb(75, 75, 11), Color.FromArgb(83, 83, 11), Color.FromArgb(91, 91, 11), Color.FromArgb(99, 99, 11), Color.FromArgb(107, 107, 15), Color.FromArgb(7, 0, 0), Color.FromArgb(15, 0, 0), Color.FromArgb(23, 0, 0), Color.FromArgb(31, 0, 0), Color.FromArgb(39, 0, 0), Color.FromArgb(47, 0, 0), Color.FromArgb(55, 0, 0), Color.FromArgb(63, 0, 0), Color.FromArgb(71, 0, 0), Color.FromArgb(79, 0, 0), Color.FromArgb(87, 0, 0), Color.FromArgb(95, 0, 0), Color.FromArgb(103, 0, 0), Color.FromArgb(111, 0, 0), Color.FromArgb(119, 0, 0), Color.FromArgb(127, 0, 0), Color.FromArgb(19, 19, 0), Color.FromArgb(27, 27, 0), Color.FromArgb(35, 35, 0), Color.FromArgb(47, 43, 0), Color.FromArgb(55, 47, 0), Color.FromArgb(67, 55, 0), Color.FromArgb(75, 59, 7), Color.FromArgb(87, 67, 7), Color.FromArgb(95, 71, 7), Color.FromArgb(107, 75, 11), Color.FromArgb(119, 83, 15), Color.FromArgb(131, 87, 19), Color.FromArgb(139, 91, 19), Color.FromArgb(151, 95, 27), Color.FromArgb(163, 99, 31), Color.FromArgb(175, 103, 35), Color.FromArgb(35, 19, 7), Color.FromArgb(47, 23, 11), Color.FromArgb(59, 31, 15), Color.FromArgb(75, 35, 19), Color.FromArgb(87, 43, 23), Color.FromArgb(99, 47, 31), Color.FromArgb(115, 55, 35), Color.FromArgb(127, 59, 43), Color.FromArgb(143, 67, 51), Color.FromArgb(159, 79, 51), Color.FromArgb(175, 99, 47), Color.FromArgb(191, 119, 47), Color.FromArgb(207, 143, 43), Color.FromArgb(223, 171, 39), Color.FromArgb(239, 203, 31), Color.FromArgb(255, 243, 27), Color.FromArgb(11, 7, 0), Color.FromArgb(27, 19, 0), Color.FromArgb(43, 35, 15), Color.FromArgb(55, 43, 19), Color.FromArgb(71, 51, 27), Color.FromArgb(83, 55, 35), Color.FromArgb(99, 63, 43), Color.FromArgb(111, 71, 51), Color.FromArgb(127, 83, 63), Color.FromArgb(139, 95, 71), Color.FromArgb(155, 107, 83), Color.FromArgb(167, 123, 95), Color.FromArgb(183, 135, 107), Color.FromArgb(195, 147, 123), Color.FromArgb(211, 163, 139), Color.FromArgb(227, 179, 151), Color.FromArgb(171, 139, 163), Color.FromArgb(159, 127, 151), Color.FromArgb(147, 115, 135), Color.FromArgb(139, 103, 123), Color.FromArgb(127, 91, 111), Color.FromArgb(119, 83, 99), Color.FromArgb(107, 75, 87), Color.FromArgb(95, 63, 75), Color.FromArgb(87, 55, 67), Color.FromArgb(75, 47, 55), Color.FromArgb(67, 39, 47), Color.FromArgb(55, 31, 35), Color.FromArgb(43, 23, 27), Color.FromArgb(35, 19, 19), Color.FromArgb(23, 11, 11), Color.FromArgb(15, 7, 7), Color.FromArgb(187, 115, 159), Color.FromArgb(175, 107, 143), Color.FromArgb(163, 95, 131), Color.FromArgb(151, 87, 119), Color.FromArgb(139, 79, 107), Color.FromArgb(127, 75, 95), Color.FromArgb(115, 67, 83), Color.FromArgb(107, 59, 75), Color.FromArgb(95, 51, 63), Color.FromArgb(83, 43, 55), Color.FromArgb(71, 35, 43), Color.FromArgb(59, 31, 35), Color.FromArgb(47, 23, 27), Color.FromArgb(35, 19, 19), Color.FromArgb(23, 11, 11), Color.FromArgb(15, 7, 7), Color.FromArgb(219, 195, 187), Color.FromArgb(203, 179, 167), Color.FromArgb(191, 163, 155), Color.FromArgb(175, 151, 139), Color.FromArgb(163, 135, 123), Color.FromArgb(151, 123, 111), Color.FromArgb(135, 111, 95), Color.FromArgb(123, 99, 83), Color.FromArgb(107, 87, 71), Color.FromArgb(95, 75, 59), Color.FromArgb(83, 63, 51), Color.FromArgb(67, 51, 39), Color.FromArgb(55, 43, 31), Color.FromArgb(39, 31, 23), Color.FromArgb(27, 19, 15), Color.FromArgb(15, 11, 7), Color.FromArgb(111, 131, 123), Color.FromArgb(103, 123, 111), Color.FromArgb(95, 115, 103), Color.FromArgb(87, 107, 95), Color.FromArgb(79, 99, 87), Color.FromArgb(71, 91, 79), Color.FromArgb(63, 83, 71), Color.FromArgb(55, 75, 63), Color.FromArgb(47, 67, 55), Color.FromArgb(43, 59, 47), Color.FromArgb(35, 51, 39), Color.FromArgb(31, 43, 31), Color.FromArgb(23, 35, 23), Color.FromArgb(15, 27, 19), Color.FromArgb(11, 19, 11), Color.FromArgb(7, 11, 7), Color.FromArgb(255, 243, 27), Color.FromArgb(239, 223, 23), Color.FromArgb(219, 203, 19), Color.FromArgb(203, 183, 15), Color.FromArgb(187, 167, 15), Color.FromArgb(171, 151, 11), Color.FromArgb(155, 131, 7), Color.FromArgb(139, 115, 7), Color.FromArgb(123, 99, 7), Color.FromArgb(107, 83, 0), Color.FromArgb(91, 71, 0), Color.FromArgb(75, 55, 0), Color.FromArgb(59, 43, 0), Color.FromArgb(43, 31, 0), Color.FromArgb(27, 15, 0), Color.FromArgb(11, 7, 0), Color.FromArgb(0, 0, 255), Color.FromArgb(11, 11, 239), Color.FromArgb(19, 19, 223), Color.FromArgb(27, 27, 207), Color.FromArgb(35, 35, 191), Color.FromArgb(43, 43, 175), Color.FromArgb(47, 47, 159), Color.FromArgb(47, 47, 143), Color.FromArgb(47, 47, 127), Color.FromArgb(47, 47, 111), Color.FromArgb(47, 47, 95), Color.FromArgb(43, 43, 79), Color.FromArgb(35, 35, 63), Color.FromArgb(27, 27, 47), Color.FromArgb(19, 19, 31), Color.FromArgb(11, 11, 15), Color.FromArgb(43, 0, 0), Color.FromArgb(59, 0, 0), Color.FromArgb(75, 7, 0), Color.FromArgb(95, 7, 0), Color.FromArgb(111, 15, 0), Color.FromArgb(127, 23, 7), Color.FromArgb(147, 31, 7), Color.FromArgb(163, 39, 11), Color.FromArgb(183, 51, 15), Color.FromArgb(195, 75, 27), Color.FromArgb(207, 99, 43), Color.FromArgb(219, 127, 59), Color.FromArgb(227, 151, 79), Color.FromArgb(231, 171, 95), Color.FromArgb(239, 191, 119), Color.FromArgb(247, 211, 139), Color.FromArgb(167, 123, 59), Color.FromArgb(183, 155, 55), Color.FromArgb(199, 195, 55), Color.FromArgb(231, 227, 87), Color.FromArgb(0, 255, 0), Color.FromArgb(171, 231, 255), Color.FromArgb(215, 255, 255), Color.FromArgb(103, 0, 0), Color.FromArgb(139, 0, 0), Color.FromArgb(179, 0, 0), Color.FromArgb(215, 0, 0), Color.FromArgb(255, 0, 0), Color.FromArgb(255, 243, 147), Color.FromArgb(255, 247, 199), Color.FromArgb(255, 255, 255), Color.FromArgb(159, 91, 83), };
		}

		public override Bitmap BuildFaceLightmap(int p, int w, int h)
		{
			var b = new Bitmap(w, h);
			for (int y = 0; y < h; ++y)
				for (int x = 0; x < w; ++x)
				{
					b.SetPixel(x, y, Color.FromArgb(lightmap[p], lightmap[p+1], lightmap[p+2]));
					p+=3;
				}
			return b;
		}

		public override Color[] GetPalette(BinaryReader source, miptex_t tex)
		{
			var p = source.BaseStream.Position;
			int offset = (((int)tex.width * (int)tex.height) * 85) >> 6;
			
			source.BaseStream.Seek(2+offset, SeekOrigin.Current);
			var b = source.ReadBytes(256*3);
			var c = new Color[256];
			for (int i = 0; i < 256; ++i)
			{
				c[i] = Color.FromArgb(255, b[i * 3], b[i * 3 + 1], b[i * 3 + 2]);
			}
			source.BaseStream.Seek(p, SeekOrigin.Begin);
			return c;
		}
	}
}

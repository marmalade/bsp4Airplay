using System.Text;

namespace BspFileFormat.Q3
{
	public class texture_t
	{
		public string name;
		public uint flags;
		public uint contents;

		public void Read(System.IO.BinaryReader source)
		{
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[]{' ','\0'});
			flags = source.ReadUInt32();
			contents = source.ReadUInt32();
		}

		public bool IsSolid
		{
			get
			{
				return 0 == (flags & SURF_NONSOLID);
			}
		}

		public const uint SURF_NODAMAGE = 0x1;		// never give falling damage
public const uint SURF_SLICK		=		0x2;		// effects game physics
public const uint SURF_SKY			=	0x4;		// lighting from environment map
public const uint SURF_LADDER		=		0x8;
public const uint SURF_NOIMPACT		=	0x10;	// don't make missile explosions
public const uint SURF_NOMARKS		=	0x20;	// don't leave missile marks
public const uint SURF_FLESH		=		0x40;	// make flesh sounds and effects
public const uint SURF_NODRAW		=		0x80;	// don't generate a drawsurface at all
public const uint SURF_HINT			=	0x100;	// make a primary bsp splitter
public const uint SURF_SKIP			=	0x200;	// completely ignore, allowing non-closed brushes
public const uint SURF_NOLIGHTMAP	=		0x400;	// surface doesn't need a lightmap
public const uint SURF_POINTLIGHT	=		0x800;	// generate lighting info at vertexes
public const uint SURF_METALSTEPS	=		0x1000;	// clanking footsteps
public const uint SURF_NOSTEPS		=	0x2000;	// no footstep sounds
public const uint SURF_NONSOLID		=	0x4000;	// don't collide against curves with this set
public const uint SURF_LIGHTFILTER	=	0x8000;	// act as a light filter during q3map -light
public const uint SURF_ALPHASHADOW	=	0x10000;	// do per-pixel light shadow casting in q3map
public const uint SURF_NODLIGHT		=	0x20000;	// don't dlight even if solid (solid lava, skies)
public const uint SURF_DUST			=	0x40000; // leave a dust trail when walking on this surface

		//public const uint SURFACE_HINT = (1 << 5);       // Make a primary BSP splitter
		//public const uint SURFACE_DISCRETE = (1 << 6);       // Don't clip or merge this surface
		//public const uint SURFACE_PORTALSKY = (1 << 7);       // This surface needs a portal sky render
		//public const uint SURFACE_SLICK = (1 << 8);       // Entities should slide along this surface
		//public const uint SURFACE_BOUNCE = (1 << 9);      // Entities should bounce off this surface
		//public const uint SURFACE_LADDER = (1 << 10);      // Players can climb up this surface
		//public const uint SURFACE_NODAMAGE = (1 << 11);      // Never give falling damage
		//public const uint SURFACE_NOIMPACT = (1 << 12);      // Don't make impact effects
		//public const uint SURFACE_NOSTEPS = (1 << 13);      // Don't play footstep sounds
		//public const uint SURFACE_NOCLIP = (1 << 14);      // Don't generate a clip-surface at all
		//public const uint SURFACE_NODRAW = (1 << 15);      // Don't generate a draw-surface at all
		//public const uint SURFACE_NOTJUNC = (1 << 16);       // Don't use this surface for T-Junction fixing
	}
}

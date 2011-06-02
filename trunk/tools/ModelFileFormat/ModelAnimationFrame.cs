using System.Collections.Generic;

namespace ModelFileFormat
{
	public class ModelAnimationFrame
	{
		public float Time { get; set; }
		public List<ModelAnimationFrameBone> Bones = new List<ModelAnimationFrameBone>();
	}
}

using System.Collections.Generic;

namespace ModelFileFormat
{
	public class ModelAnimation
	{
		public string Name { get; set; }
		public List<ModelAnimationFrame> Frames = new List<ModelAnimationFrame>();
	}
}

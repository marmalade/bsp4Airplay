using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL2
{
	/// <summary>
	/// File format is here: http://developer.valvesoftware.com/wiki/MDL
	/// </summary>
	class MdlReader : IModelReader
	{
		private long startOfTheFile;

		#region IModelReader Members

		public void ReadModel(BinaryReader source, ModelDocument dest)
		{
			startOfTheFile = source.BaseStream.Position;
			//ReadHeader(source);
			//ReadTextures(source);
			//ReadBones(source);
			//ReadBodyParts(source);
			//ReadAnimations(source);

			//foreach (var bp in bodyParts)
			//    dest.Meshes.Add(BuildMesh(bp));
			//BuildAnimations(dest);
			//BuildBones(dest);
		}

		#endregion
	}
}

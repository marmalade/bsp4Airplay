using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModelFileFormat.HL1
{
	public class header_t
	{
		public int id;
		public int version;
		public string name;
		public int length;
		public float[] eyeposition = new float[3];
		public float[] min = new float[3];
		public float[] max = new float[3];
		public float[] bbmin = new float[3];
		public float[] bbmax = new float[3];
		public int flags;

		public int numbones;                  // Bones
		public int boneindex;
		public int numbonecontrollers;       // Bone controllers
		public int bonecontrollerindex;

		public int numhitboxes;               // Complex bounding boxes
		public int hitboxindex;

		public int numseq;                    // Animation sequences
		public int seqindex;

		public int numseqgroups;              // Demand loaded sequences
		public int seqgroupindex;

		public int numtextures;               // Raw textures
		public int textureindex;
		public int texturedataindex;

		public int numskinref;                // Replaceable textures
		public int numskinfamilies;
		public int skinindex;

		public int numbodyparts;
		public int bodypartindex;

		public int numattachments;            // Queryable attachable points
		public int attachmentindex;

		public int soundtable;
		public int soundindex;

		public int soundgroups;
		public int soundgroupindex;

		public int numtransitions;            /* Animation node to animation
                                                                 * node transition graph */
		public int transitionindex;

		public void Read(BinaryReader source)
		{
			id = source.ReadInt32();
			version = source.ReadInt32();
			name = Encoding.ASCII.GetString(source.ReadBytes(64)).Trim(new char[]{'\0'});
			length = source.ReadInt32();
			eyeposition[0] = source.ReadSingle();
			eyeposition[1] = source.ReadSingle();
			eyeposition[2] = source.ReadSingle();
			min[0] = source.ReadSingle();
			min[1] = source.ReadSingle();
			min[2] = source.ReadSingle();
			max[0] = source.ReadSingle();
			max[1] = source.ReadSingle();
			max[2] = source.ReadSingle();
			bbmin[0] = source.ReadSingle();
			bbmin[1] = source.ReadSingle();
			bbmin[2] = source.ReadSingle();
			bbmax[0] = source.ReadSingle();
			bbmax[1] = source.ReadSingle();
			bbmax[2] = source.ReadSingle();
			flags = source.ReadInt32();
			numbones = source.ReadInt32();
			boneindex = source.ReadInt32();
			numbonecontrollers = source.ReadInt32();
			bonecontrollerindex = source.ReadInt32();
			numhitboxes = source.ReadInt32();
			hitboxindex = source.ReadInt32();
			numseq = source.ReadInt32();
			seqindex = source.ReadInt32();
			numseqgroups = source.ReadInt32();
			seqgroupindex = source.ReadInt32();
			numtextures = source.ReadInt32();
			textureindex = source.ReadInt32();
			texturedataindex = source.ReadInt32();
			numskinref = source.ReadInt32();
			numskinfamilies = source.ReadInt32();
			skinindex = source.ReadInt32();
			numbodyparts = source.ReadInt32();
			bodypartindex = source.ReadInt32();
			numattachments = source.ReadInt32();
			attachmentindex = source.ReadInt32();
			soundtable = source.ReadInt32();
			soundindex = source.ReadInt32();
			soundgroups = source.ReadInt32();
			soundgroupindex = source.ReadInt32();
			numtransitions = source.ReadInt32();
			transitionindex = source.ReadInt32();
		}
	}
}

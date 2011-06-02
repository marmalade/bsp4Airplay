using System;
using System.Collections.Generic;
using System.Text;
using BspFileFormat.Utils;
using ReaderUtils;

namespace BspFileFormat.Q1HL1
{
	public class clipnode_t
	{
		uint planenum;             // The plane which splits the node
		short front;                 // If positive, id of Front child node
		// If -2, the Front part is inside the model
		// If -1, the Front part is outside the model
		short back;                  // If positive, id of Back child node
		// If -2, the Back part is inside the model
		// If -1, the Back part is outside the model
	}
}

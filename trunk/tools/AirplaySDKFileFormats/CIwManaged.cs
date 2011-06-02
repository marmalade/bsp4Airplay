using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaySDKFileFormats
{

	public class CIwManaged : CIwParseable
	{
		string name;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				SetName(value);
			}
		}

		/// <summary>
		/// Set name (hashed value) 
		/// </summary>
		/// <param name="pName"> pName  the hashed name of the managed object  </param>
		public virtual void SetName(string pName)
		{
			name = pName;
		}

		public override void WrtieBodyToStream(CTextWriter writer)
		{
			base.WrtieBodyToStream(writer);
			if (this.Name != null)
				writer.WriteString("name", this.Name);
		}

	}
}

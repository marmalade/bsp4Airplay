using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace AirplaySDKFileFormats
{
	public class CIwManagedList : IEnumerable<CIwManaged>
	{
		List<CIwManaged> list = new List<CIwManaged>();

		public void Add(CIwManaged pObject)
		{
			list.Add(pObject);
		}
		public void Clear()
		{
			list.Clear();
		}
		public void Push(CIwManaged pObject)
		{
			list.Add(pObject);
		}

		#region IEnumerable<CIwManaged> Members

		IEnumerator<CIwManaged> IEnumerable<CIwManaged>.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList)list).GetEnumerator();
		}

		#endregion
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.crypt
{	
	/// <summary>
	/// 加密與解密金鑰
	/// </summary>
	///
	public class LineageKeys
	{
		public long[] encodeKey;
	
		public long[] decodeKey;
		
		public LineageKeys()
		{
			this.encodeKey = new long[] { 0, 0 };
			this.decodeKey = new long[] { 0, 0 };
		}
	
	}
}

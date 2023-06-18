using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.crypt.exception
{
	[Serializable]
	public class NoSuchClientId : Exception
	{	
		private const long serialVersionUID = 1L;
	}
}

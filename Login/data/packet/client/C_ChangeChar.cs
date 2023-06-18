using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.client
{	
	/// <summary>
	/// 要求切換角色
	/// </summary>
	///
	public class C_ChangeChar : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
			    AllSave.USER = null;
			    return true;
			}
			catch {}
			
			return false;
		}
	
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.client
{	
	/// <summary>
	/// 要求登陸遊戲服務器
	/// </summary>
	///
	public class C_LoginToServer: BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				if (AllSave.USER == null)
				{
					AllSave.USER = new UserPc();
				}
				
			    return true;
			}
			catch {}
			
			return false;
		}
	
	}
}

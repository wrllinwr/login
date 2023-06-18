using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送服務器提示信息封包
	/// </summary>
	public class S_ServerMessage : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try {
				// 待處理
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送角色自身屬性封包
	/// </summary>
	public class S_OwnCharStatus : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try {
				this.Read(abyte0);
	
				int objid = this.ReadD(); // 參考值:objid	
				if (AllSave.USER == null)
				{
					AllSave.USER = new UserPc();
				}
				AllSave.USER.OBJID = objid;
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

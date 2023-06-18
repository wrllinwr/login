using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送角色外形更新封包
	/// </summary>
	public class S_CharVisualUpdate : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				this.Read(abyte0);
	
				int objid = this.ReadD(); // OBJID
				int weaponType = this.ReadC(); // 武器模式
	
				if (objid == AllSave.USER.OBJID)
				{
					AllSave.USER.WEAPONTYPE = weaponType;
				}
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送二段加速封包
	/// </summary>
	public class S_SkillBrave : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try {
				this.Read(abyte0);
	
				int objid = this.ReadD();
				int type = this.ReadC();
	
				if (objid == AllSave.USER.OBJID)
				{
					AllSave.USER.BRAVESPEED = type;
				}
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

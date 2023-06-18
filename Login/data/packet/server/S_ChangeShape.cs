using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送變身封包
	/// </summary>
	public class S_ChangeShape : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				this.Read(abyte0);
	
				int objid = this.ReadD();
				int polyId = this.ReadH();
	
				if (objid == AllSave.USER.OBJID)
				{
					AllSave.USER.GFXID = polyId;
				}
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

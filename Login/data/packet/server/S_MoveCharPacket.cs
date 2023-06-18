using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送移動封包
	/// </summary>
	public class S_MoveCharPacket : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				this.Read(abyte0);
	
				int objid = this.ReadD(); // cha.getId()
				int x = this.ReadH(); // x
				int y = this.ReadH(); // y
				int heading = this.ReadC(); // cha.getHeading()
	
				if (objid == AllSave.USER.OBJID)
				{
					AllSave.USER.X = x;
					AllSave.USER.Y = y;
					AllSave.USER.HEADING = heading;
				}
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

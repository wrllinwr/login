using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;
using SFL.data.plugin;

namespace SFL.data.packet.client
{	
	/// <summary>
	/// 要求移動角色
	/// </summary>
	///
	public class C_MoveChar : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				if (Program.IS_CHECK_SPEED)
				{
					int result = AllSave.MOVESPEED.CheckInterval(SpeedCheck.ACT_TYPE.MOVE);
					if (result == SpeedCheck.R_DISCONNECTED)
					{
						return false;
					}
				}
				
			    return true;
			}
			catch {}
			
			return false;
		}
	
	}
}

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
	/// 要求遠距離攻擊
	/// </summary>
	///
	public class C_AttackBow : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				if (Program.IS_CHECK_SPEED)
				{
					int result = AllSave.MOVESPEED.CheckInterval(SpeedCheck.ACT_TYPE.ATTACK);
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

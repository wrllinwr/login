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
	/// 要求使用魔法
	/// </summary>
	///
	public class C_UseSkill : BasePacket
	{	
		private static int[] ATTACKSKILLS = { 
		    4, 6, 7, 10, 15, 16, 17, 22, 25, 28, 30, 34, 38,                                       
		    45, 46, 50, 53, 58, 59, 62, 65, 70, 74, 77, 80,                                        
		    108, 132, 184, 187, 189, 192, 194, 202, 203, 207,                                        
		    208, 213 };
		
		public override bool Execute(byte[] abyte0)
		{
			try
			{	
				if (Program.IS_CHECK_SPEED)
				{
					int result = 0;
					
					this.Read(abyte0);
					
					int row = this.ReadC();
					
					int column = this.ReadC();
					
					int skillId = (row << 3) + column + 1;
					
					foreach (int skillid in ATTACKSKILLS)
					{
						if (skillid == skillId)
						{
							result = AllSave.MOVESPEED.CheckInterval(SpeedCheck.ACT_TYPE.SPELL_DIR);
						}
						else
						{
							result = AllSave.MOVESPEED.CheckInterval(SpeedCheck.ACT_TYPE.SPELL_NODIR);
						}
					}
					
					if (result == SpeedCheck.R_DISCONNECTED)
					{
						return false;
					}
				}
				
			    return true;
			}
			catch {}
			finally
			{
				this.over();
			}
			
			return false;
		}
	
	}
}

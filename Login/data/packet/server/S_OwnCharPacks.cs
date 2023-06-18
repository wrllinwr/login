using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送角色信息封包
	/// </summary>
	public class S_OwnCharPacks : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				this.Read(abyte0);
	
				int x = this.ReadH(); // X座標
				int y = this.ReadH(); // Y座標
				int objid = this.ReadD(); // OBJID
	
				if (objid == AllSave.USER.OBJID)
				{
					int gfx = this.ReadH(); // 外型
					int gfxStatus = this.ReadC(); // 狀態	
					int heading = this.ReadC(); // 面向
					int light = this.ReadC(); // 亮度
					int moveSpeed = this.ReadC(); // 移動速度
					this.ReadD(); // 經驗質
					this.ReadH(); // 正義質	
					String name = this.ReadS(); // 名稱
	
					AllSave.USER.X = x;
					AllSave.USER.Y = y;
					AllSave.USER.HEADING = heading;
					AllSave.USER.LIGHT = light;
					AllSave.USER.GFXID = gfx;
					AllSave.USER.WEAPONTYPE = gfxStatus;
					AllSave.USER.NAME = name;	
					AllSave.USER.MOVESPEED = moveSpeed;	
				}
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

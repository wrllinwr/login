using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送角色選擇界面的玩家清單
	/// </summary>
	public class S_CharList : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				this.Read(abyte0); // 取得人物資料	
				
				String name = this.ReadS();//Name(人物名稱)	
				UserPc userList = new UserPc();
				userList.NAME = name;
				AllSave.USERLIST.Add(userList);	
				
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

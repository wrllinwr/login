using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.temp;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送新建角色封包
	/// </summary>
	public class S_NewCharPacket : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				this.Read(abyte0);
	
				String name = this.ReadS(); // Name(人物名稱)
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

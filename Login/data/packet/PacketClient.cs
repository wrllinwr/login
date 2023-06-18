using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.packet
{
	/// <summary>
	/// 客戶端封包處理
	/// </summary>
	///
	public class PacketClient : OpLoad
	{
		/// <summary>
		/// 客戶端封包處理
		/// </summary>
		///
		/// <param name="encrypt">客戶端傳來的封包</param>
		/// <exception cref="System.Exception">拋出處理異常</exception>
		public bool HandlePacket(byte[] encrypt)
		{
			int opcode = encrypt[0] & 0xff;
			if (OpLoad.COPLIST.ContainsKey(opcode))
			{
			    BasePacket exe = OpLoad.COPLIST[opcode];			
			    if (exe != null)
			    {
				    return exe.Execute(encrypt);	
			    }
			}
			
			return true;
		}
		
	}
}

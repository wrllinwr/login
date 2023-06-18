using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.packet
{	
	/// <summary>
	/// 服務端封包處理
	/// </summary>
	///
	public class PacketServer : OpLoad
	{	
		/// <summary>
		/// 服務端封包處理
		/// </summary>
		///
		/// <param name="encrypt">服務端傳來的封包</param>
		/// <exception cref="System.Exception">拋出處理異常</exception>
		public void HandlePacket(byte[] decrypt)
		{
			int opcode = decrypt[0] & 0xff;
			if (OpLoad.SOPLIST.ContainsKey(opcode))
			{
			    BasePacket exe = OpLoad.SOPLIST[opcode];
			    if (exe != null)
			    {
				    exe.Execute(decrypt);
			    }
			}
		}
	
	}
}

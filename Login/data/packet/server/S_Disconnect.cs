using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using SFL.data.thread;

namespace SFL.data.packet.server
{
	/// <summary>
	/// 發送斷線封包
	/// </summary>
	public class S_Disconnect : BasePacket
	{	
		public override bool Execute(byte[] abyte0)
		{
			try
			{
				DisConTime disTime = new DisConTime();
				Thread dsThread = new Thread(new ThreadStart(disTime.Run));
				dsThread.Start();
	            return true;
			}
			catch {}
			
			return false;	
		}
	
	}
}

using System;
using System.Threading;

namespace SFL.data.thread
{
	/// <summary>
	/// 斷線時間緩衝
	/// </summary>
	public class DisConTime
	{
		public void Run()
		{
			try
			{			
				Thread.Sleep(2000); // 2秒後切斷連接
				Console.WriteLine("DisConTime.Run处调用KillGame");
				Program.GetForm1().KillGame();
			}
			catch {}
		}
	}
}

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SFL.data.plugin
{
	/// <summary>
	/// Description of util.
	/// </summary>
	public class Util
	{
		/// <summary>
		/// 獲得文件的MD5值
		/// </summary>
		/// <param name="fileName">文件名字</param>
		/// <returns>MD5值</returns>
		///
		public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("X2"));
                }

                return sb.ToString();
            }
            catch {}

            return null;
        }
		
		/// <summary>
		/// 將文件寫入內存
		/// </summary>
		/// 
		/// <param name="oriFileName">文件名字</param>
		/// <returns>內存信息</returns>
		/// 
		public static MemoryStream ConvertFileToMemory(string oriFileName)
		{
			StreamReader oriFileSr = new StreamReader(oriFileName); // 讀取流
			string allContent = oriFileSr.ReadToEnd(); // 全部讀取			
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(allContent)); // 放入內存
			oriFileSr.Close();
			
			return ms;
		}
		
		/// <summary>
		/// 程序自我更新
		/// </summary>
		/// 
		public static void KillSelfThenRun()
		{	
			string killBatFiles = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kill.bat"); // kill文件路徑
			// 生成kill.bat文件
			using (StreamWriter swKill = File.CreateText(killBatFiles))
			{
				string oriFileName = "npklogin0.sys";
				swKill.WriteLine(string.Format(@"@echo off" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + ":selfkill" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "attrib -a -r -s -h " + "{0}" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "del " + "{0}" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "if exist " + "{0}" + " goto selfkill" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "copy /y " + oriFileName + " " + "{0}" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "del " + oriFileName + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "start " + "{0}" + Environment.NewLine
　　　　　　　　　　　　　　　　　　　　　　　 + "del kill.bat", AppDomain.CurrentDomain.FriendlyName));
			}
			// 啟動自刪除批處理文件
			ProcessStartInfo info = new ProcessStartInfo(killBatFiles);
			info.WindowStyle = ProcessWindowStyle.Hidden;
			Process.Start(info);

			// 強制關閉當前進程
			Console.WriteLine("Utils 退出");
			Environment.Exit(0);
		}

	}
}

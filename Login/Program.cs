using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using SFL.data.plugin;
using SFL.Properties;

namespace SFL
{
	public static class Program
	{
		public static bool IS_CHECK_SPEED = false; // 是否进行速度检测
		
		private static MemoryStream _md5Ms; // 初始md5值獲取
		
		private static string _mainMd5; // 登陸器主程序MD5值
		
		private static string _serverInfoMd5; // 服務器配置文件的Md5值
		
		private static string _sprMd5; // spr配置文件的Md5值
		
		private static string _botMd5; // bot配置文件的Md5值
		
		private static string _iniFile; // 配置文件
		
		private static string _bulletin; // 公告地址
		
		private static string _serverlist; // 服務器列表
		
		private static string _button; // 按鈕
		
		private static bool _skinEnable; // 登陸器皮膚
		
		private static string _protocol; // 未知
		
		private static string _processes; // 禁止的程序進程
		
		// private static bool _showForm; // 是否顯示窗口
		
		private static string _showURL; // 第一個窗口顯示的url
		
		private static Form1 _form1; // 內窗口實例
		
		private static Form2 _form2; // 第一窗口實例
		
		/// <summary>
		/// 取得服務器配置文件的Md5值
		/// </summary>
		public static string GetServerInfoMd5()
		{
			return _serverInfoMd5;
		}
		
		/// <summary>
		/// 取得spr配置文件的Md5值
		/// </summary>
		public static string GetSprMd5()
		{
			return _sprMd5;
		}
		
		/// <summary>
		/// 取得bot配置文件的Md5值
		/// </summary>
		public static string GetBotMd5()
		{
			return _botMd5;
		}

		/// <summary>
		/// 取得應用程式的配置文件名
		/// </summary>
		public static string GetIniFile()
		{
			return _iniFile;
		}
		
		/// <summary>
		/// 取得登陸器初始界面的公告地址
		/// </summary>
		public static string GetBulletin()
		{
			return _bulletin;
		}
		
		/// <summary>
		/// 取得登陸器的服務器列表
		/// </summary>
		public static string GetServerlist()
		{
			return _serverlist;
		}
		
		/// <summary>
		/// 取得登陸器的按鈕
		/// </summary>
		public static string GetButton()
		{
			return _button;
		}
		
		/// <summary>
		/// 是否開啟登陸器的皮膚
		/// </summary>
		public static bool IsSkinEnable()
		{
			return _skinEnable;
		}
		
		/// <summary>
		/// 未知用途
		/// </summary>
		public static string getProtocol()
		{
			return _protocol;
		}
		
		/// <summary>
		/// 取得禁止的進程
		/// </summary>
		public static string getProcesses()
		{
			return _processes;
		}
		
		/// <summary>
		/// 取得FORM1實例
		/// </summary>
		public static Form1 GetForm1()
		{
			return _form1;
		}
		
		/// <summary>
		/// Md5信息讀入
		/// </summary>
		///
		private static void LoadMD5()
		{
			StreamReader sr = new StreamReader(_md5Ms); // 讀取流
			string str;
			string runwork = null;
			
			try
			{
				while ((str = sr.ReadLine()) != null)
				{
					// 此段落為註解
					if (str.StartsWith("#"))
					{
						continue;
					}
					
					if (str.StartsWith("["))
					{
						runwork = str;
						continue;
					}
					
					if (runwork == null)
					{
						continue;
					}
					else
					{
						// 取值的時候去除"Key="
						if (runwork.Equals("[main]"))
						{
							_mainMd5 = str.Remove(0, 4);
						}
						else if (runwork.Equals("[server]"))
						{
							_serverInfoMd5 = str.Remove(0, 4);
						}
						else if (runwork.Equals("[spr]"))
						{
							_sprMd5 = str.Remove(0, 4);
						}
						else if (runwork.Equals("[bot]"))
						{
							_botMd5 = str.Remove(0, 4);
						}

					}
				}
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				sr.Close();
			}
		}
		
		/// <summary>
		/// 服務器信息讀入
		/// </summary>
		///
		private static void LoadServerInfo()
		{
			_iniFile = Directory.GetCurrentDirectory() + "\\" + "npklogin1.sys";
			// 檢查文件是否存在
			if (!File.Exists(_iniFile))
			{
				MessageBox.Show("配置文件(1)缺失，請重新下載登陸器！", "天堂");
                Console.WriteLine("Program.LoadServerInfo处读取npklogin1.sys 退出");
                Environment.Exit(0);
				return;
			}
			
			// 校驗MD5值
			byte[] md5bytes = Convert.FromBase64String(_serverInfoMd5);
			_serverInfoMd5 = Encoding.UTF8.GetString(md5bytes, 0, md5bytes.Length);
			if (!Util.GetMD5HashFromFile(_iniFile).Equals(_serverInfoMd5))
			{
				MessageBox.Show("配置文件(1)讀取錯誤，請重新下載登陸器！", "天堂");
                Console.WriteLine("Programs.LoadServerInfo处校验MD5 退出");
                Environment.Exit(0);
				return;
			}
			
			File.SetAttributes(_iniFile, FileAttributes.Normal);
			_bulletin = Win32API.GetINI("Background", "Bulletin", "", _iniFile);
			_serverlist = Win32API.GetINI("Background", "Serverlist", "", _iniFile);
			_button = Win32API.GetINI("Background", "Button", "", _iniFile);
			_skinEnable = Convert.ToBoolean(Win32API.GetINI_Int("Background", "Enable", 0, _iniFile));
			_protocol = Win32API.GetINI("Background", "Protocol", "", _iniFile);
			_processes = Win32API.GetINI("KillProc", "ProcName", "", _iniFile);
		}
		
		/// <summary>
		/// 檢測Lin.bin進程
		/// </summary>
		///
		public static bool CheckLinProcess()
		{
			try
			{
				int linProcNum = 0;
				IntPtr dlgHandle = Win32API.GetWindow(Win32API.GetDesktopWindow(), Win32API.GetWindow_Cmd.GW_CHILD);
				while (dlgHandle != IntPtr.Zero)
				{
					string title = Win32API.GetWindowText(dlgHandle);
					string className = Win32API.GetClassName(dlgHandle);
					if (title.Equals("Lineage Windows Client (12011702)") && className.Equals("Lineage"))
					{
						++ linProcNum;
					}
					
					if (linProcNum >= 2)
					{
						return false;
					}

					dlgHandle = Win32API.GetWindow(dlgHandle, Win32API.GetWindow_Cmd.GW_HWNDNEXT);
				}
			}
			catch
			{
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// 關閉非法線程
		/// </summary>
		///
		private static void KillProcess()
		{
			string[] textArray1 = _processes.Split(new char[] { ',' });
			Process[] processArray1 = Process.GetProcesses();
			for (int i = 0; i < textArray1.Length; i++)
			{
				Process[] processArray2 = processArray1;
				for (int j = 0; j < processArray2.Length; j++)
				{
					Process process1 = processArray2[j];
					if (process1.ProcessName.ToUpper() == textArray1[i].ToString().ToUpper().Trim())
					{
						process1.Kill();
					}
				}
			}
		}
		
		/// <summary>
		/// 載入配置文件
		/// </summary>
		///
		private static void LoadIniSet()
		{
			try
			{
				LoadServerInfo(); // 載入serverinfo檔案
				SprData.GetInstance().Load(); // 載入spr檔案
				AntiBotData.GetInstance().Load(); // 載入防外掛信息
			}
			catch
			{
                Console.WriteLine("Programs.LoadIniSet 退出");
                Environment.Exit(0);
				return;
			}
			
			KillProcess(); // 禁止的進程掃瞄
		}
		
		private static void CheckMd5()
		{
			string file0 = Directory.GetCurrentDirectory() + "\\" + AppDomain.CurrentDomain.FriendlyName;
			string file0Tmp = Directory.GetCurrentDirectory() + "\\tmp.bak";
			string file1 = Directory.GetCurrentDirectory() + "\\" + "npklogin1.sys";
			string file2 = Directory.GetCurrentDirectory() + "\\" + "npklogin2.sys";
			string file3 = Directory.GetCurrentDirectory() + "\\" + "npklogin3.sys";
			
			File.Delete(file0Tmp);
			File.Copy(file0, file0Tmp);
			byte[] mainMd5bytes = Convert.FromBase64String(_mainMd5);
			string mianMd5 = Encoding.UTF8.GetString(mainMd5bytes, 0, mainMd5bytes.Length);
			if (!Util.GetMD5HashFromFile(file0Tmp).Equals(mianMd5))
			{
				Ini.AddUpdateFiles("npklogin0.zip"); // 從遠程下載登陸器主文件
			}
			File.Delete(file0Tmp);
			
			// 檢查文件是否存在
			if (!File.Exists(file1))
			{
				Ini.AddUpdateFiles("npklogin1.zip"); // 從遠程下載服務器配置文件
			}
			else
			{
				byte[] md5bytes = Convert.FromBase64String(_serverInfoMd5);
				string md5 = Encoding.UTF8.GetString(md5bytes, 0, md5bytes.Length);
				if (!Util.GetMD5HashFromFile(file1).Equals(md5))
				{
					Ini.AddUpdateFiles("npklogin1.zip"); // 從遠程下載服務器配置文件
				}
			}
			
			// 檢查文件是否存在
			if (!File.Exists(file2))
			{
				Ini.AddUpdateFiles("npklogin2.zip"); // 從遠程下載服務器配置文件
			}
			else
			{
				byte[] md5bytes = Convert.FromBase64String(_botMd5);
				string md5 = Encoding.UTF8.GetString(md5bytes, 0, md5bytes.Length);
				if (!Util.GetMD5HashFromFile(file2).Equals(md5))
				{
					Ini.AddUpdateFiles("npklogin2.zip"); // 從遠程下載服務器配置文件
				}
			}
			
			// 檢查文件是否存在
			if (!File.Exists(file3))
			{
				Ini.AddUpdateFiles("npklogin3.zip"); // 從遠程下載服務器配置文件
			}
			else
			{
				byte[] md5bytes = Convert.FromBase64String(_sprMd5);
				string md5 = Encoding.UTF8.GetString(md5bytes, 0, md5bytes.Length);
				if (!Util.GetMD5HashFromFile(file3).Equals(md5))
				{
					Ini.AddUpdateFiles("npklogin3.zip"); // 從遠程下載服務器配置文件
				}
			}
		}
		
		private static bool RemoteFileExists(string fileUrl)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(fileUrl));
				httpWebRequest.Method = "HEAD";
				httpWebRequest.Timeout = 2000;
				HttpWebResponse res = (HttpWebResponse) (httpWebRequest.GetResponse());
				
				return ((res.StatusCode == HttpStatusCode.OK) && (res.ContentLength > 0));
			}
			catch
			{
				return false;
			}
		}

		public static Form2 GetForm2()
		{
			return _form2;
		}
		
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// Ini.SetLastGetUpdateTime(Settings.Default.LastUpdateTime);
			
			try
			{
				if (!CheckLinProcess())
				{
					MessageBox.Show("只允許同時開啟兩個客戶端！", "天堂");
					return;
				}
				
				for (int num = 0; num < Setting.WEBURL.Length; num ++)
				{
					if (RemoteFileExists(Setting.WEBURL[num] + "md5.txt"))
					{
						Setting.WebUrl = Setting.WEBURL[num];
						break;
					}
				}

				Console.WriteLine(Setting.WebUrl + " ");
				if (Setting.WebUrl == null || Setting.WebUrl.Equals(""))
				{
					MessageBox.Show("登录器特征码获取失败，请重开登录器！3", "天堂");
					return;
				}
				
				_md5Ms = Ini.ReadIni(Setting.WebUrl + "md5.txt");
				LoadMD5();
			}
			catch
			{
				MessageBox.Show("登录器特征码获取失败，请重开登录器！2", "天堂");
				return;
			}
			
			if (_mainMd5 == null || _serverInfoMd5 == null || _sprMd5 == null || _botMd5 == null)
			{
				MessageBox.Show("登陸器特徵碼獲取缺失無法開啟，請聯繫管理員111111！", "天堂");
				return;
			}
			
			CheckMd5();
			
			// 開始打開登錄器窗口
			string iniFile = ".\\npklogin1.sys"; // 讀取未更新的配置文件
			if (File.Exists(iniFile))
			{
				File.SetAttributes(iniFile, FileAttributes.Normal);
				string s = Win32API.GetINI("lineage", "2", "", iniFile); // 讀取顯示的網址
				byte[] bytes = Convert.FromBase64String(s);
				_showURL = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			}
			else
			{
				_showURL = "http://localhost";
			}

			_form2 = new Form2(_showURL);
			Console.WriteLine("之前" +" 0k:" + DialogResult.OK);
			if (_form2.ShowDialog() == DialogResult.OK)
			{
                Console.WriteLine("为啥");
				_form2.Dispose();
				
				LoadIniSet();
				if (_form1 == null)
				{
                    _form1 = new Form1();
					Application.Run(_form1);
				}
			}
			
		}
	}
}

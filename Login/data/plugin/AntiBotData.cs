using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using SFL.data.plugin;
using SFL.Properties;
using System.Diagnostics;

namespace SFL.data.plugin
{
	public class AntiBotData
	{
		private static MemoryStream _tmpMemory;
		
		private static ArrayList _fName = new ArrayList();
		
		private static ArrayList _pName = new ArrayList();
		
		private readonly static IDictionary<string, ArrayList> _wTitle = new Dictionary<string, ArrayList>();
		
		private static AntiBotData _instance;
	
		public static AntiBotData GetInstance()
		{
			if (_instance == null)
			{
				_instance = new AntiBotData();
			}
			
			return _instance;
		}
		
		public void Load()
		{
			try
			{
				string md5FromIni = Program.GetBotMd5();
				byte[] md5bytes = Convert.FromBase64String(md5FromIni);
                md5FromIni = Encoding.UTF8.GetString(md5bytes, 0, md5bytes.Length);
                    
				string locFile = Directory.GetCurrentDirectory() + "\\" + "npklogin2.sys";
				// 檢查文件是否存在
				if (!File.Exists(locFile))
				{
			        MessageBox.Show("配置文件(2)缺失，請重新下載登陸器！", "天堂");
					throw(new Exception());
				}
				
				// 校驗MD5值
				if (!Util.GetMD5HashFromFile(locFile).Equals(md5FromIni))
				{
					MessageBox.Show("配置文件(2)讀取錯誤，請重新下載登陸器！", "天堂");
					throw(new Exception());
				}
				
			    _tmpMemory = Util.ConvertFileToMemory(locFile); // 將文件讀入內存
				this.LoadFPWName(); // 從內存中載入資料			
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				_tmpMemory.Close(); // 關閉內存流
			}
		}
		
		/// <summary>
		/// 防外掛fName信息導入
		/// </summary>
		///
		private void LoadFPWName()
		{
            StreamReader sr = new StreamReader(_tmpMemory); // 讀取流
            string str;
            string runwork = null;
            
            try
            {
                while ((str = sr.ReadLine()) != null)
                {
                    // 解密作業
			        byte[] stbytes = Convert.FromBase64String(str);
                    str = Encoding.UTF8.GetString(stbytes, 0, stbytes.Length);
                    
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
                    	if (runwork.Equals("[fname]"))
                    	{
                    		uint value = uint.Parse(str);
                    	    _fName.Add(value);
                    	}
                    	else if (runwork.Equals("[pname]"))
                    	{
                    		uint value = uint.Parse(str);
                    	    _pName.Add(value);
                    	}
                    	else if (runwork.Equals("[wtitle]"))
                    	{
                    	    StringTokenizer wtitleSt = new StringTokenizer(str, "="); // 等號分隔符
					        string key = wtitleSt.NextToken();
					        string val = wtitleSt.NextToken();
					        
					        if (_wTitle.ContainsKey(key))
					        {
					        	_wTitle[key].Add(val);
					        }
					        else
					        {
					        	ArrayList newData = new ArrayList();
					        	newData.Add(val);
					        	_wTitle.Add(key, newData);
					        }
					        
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
		/// 取得FName的信息
		/// </summary>
		///
		public ArrayList getFName()
		{
			return _fName;
		}
		
		/// <summary>
		/// 取得PName的信息
		/// </summary>
		///
		public ArrayList getPName()
		{
			return _pName;
		}
		
		/// <summary>
		/// 取得WTitle的信息
		/// </summary>
		///
		public IDictionary<string, ArrayList> getWTitle()
		{
			return _wTitle;
		}
		
	}
}

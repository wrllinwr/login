using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SFL
{
    public class Ini
    {
        private static List<UpdateInfo> _updateFiles = new List<UpdateInfo>(); // 文件更新列表
        private static int[] _updateVersions; // 需要下載並更新的版本集合
        private static List<int> _tmpUpdateVer = new List<int>(); // 臨時的更新版本信息
        
        /// <summary>
		/// 將遠程文件讀入內存
		/// </summary>
		/// 
		/// <param name="url">遠程鏈接地址</param>
		/// <returns>內存信息</returns>
		///
        public static MemoryStream ReadIni(string url)
        {
        	WebClient client = new WebClient();
        	StreamReader oriFileSr = new StreamReader(client.OpenRead(url)); // 讀取流
			string allContent = oriFileSr.ReadToEnd(); // 全部讀取			
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(allContent)); // 放入內存
			oriFileSr.Close();
			client.Dispose();
			
			return ms;
        }

        public static void DownloadIni(string url, string savepath)
        {
            string str;
            WebClient client = new WebClient();
            
            using (StreamReader reader = new StreamReader(client.OpenRead(url)))
            {
                str = reader.ReadToEnd();
            }
            
            using (StreamWriter writer = new StreamWriter(savepath, false, Encoding.Default))
            {
                writer.Write(str);
                writer.Flush();
            }
            
            client.Dispose();
        }

        public static void SetUpdateFiles(string uri)
        {
            try
            {
            	ArrayList localVer = new ArrayList();
            	string file4 = Directory.GetCurrentDirectory() + "\\" + "npklogin4.sys";
            	if (File.Exists(file4))
            	{
            		using (StreamReader reader = new StreamReader(file4))
            		{
            			// 開始讀取本地的更新文件內容
            			while (!reader.EndOfStream)
            			{
            				string strArray = reader.ReadLine();
            				localVer.Add(int.Parse(strArray));
            			}
            			
            			reader.Close();
            		}
            	}
            	
                using (StreamReader reader = new StreamReader(uri))
                {
                    // 開始讀取遠程的更新文件內容
                    while (!reader.EndOfStream)
                    {
                        string[] strArray = reader.ReadLine().Split(new char[] { ',' }); // 文件內以逗號分割
                        int version = int.Parse(strArray[0]); // 取得版本號
                        if (localVer.Contains(version))
                        {
                        	continue;
                        }
                        
                        _tmpUpdateVer.Add(version);
                        
                        UpdateInfo update = new UpdateInfo();
                        update.UpdateVersion = version;
                        update.Folder = Setting.WebUrl + "update/" + strArray[0] + "/";
                        for (int i = 1; i < strArray.Length; i++)
                        {
                        	update.Files.Add(strArray[i] + ".zip");
                        }
                        
                        _updateFiles.Add(update); // 將文件加入更新列表
                    }
                    
                    reader.Close();
                }
            }
            catch {}
        }

        /// <summary>
		/// 將遠程文件加入更新列表
		/// </summary>
		/// 
		/// <param name="fileName">遠程文件名字</param>
		///
        public static void AddUpdateFiles(string fileName)
        {
        	try
        	{
        		UpdateInfo update = new UpdateInfo();
        		update.Folder = Setting.WebUrl;
        		update.Files.Add(fileName);
        		_updateFiles.Add(update); // 將文件加入更新列表
        	}
        	catch {}
        }
        
        public static List<UpdateInfo> GetUpdateFiles()
        {
            return _updateFiles;
        }
        
        public static List<int> GetTmpUpdateVer()
        {
        	return _tmpUpdateVer;
        }
        
        public static int[] UpdateVersions
        {
        	get
            {
                return _updateVersions;
            }
            set
            {
                _updateVersions = value;
            }
        }
        
    }
}


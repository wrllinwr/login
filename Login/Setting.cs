/*
 * 由SharpDevelop創建。
 * 用戶： Administrator
 * 日期: 2012-8-11
 * 時間: 15:58
 * 
 * 要改變這種模板請點擊 工具|選項|代碼編寫|編輯標準頭文件
 */
using System;

namespace SFL
{
	/// <summary>
	/// Description of Setting.
	/// </summary>
	public class Setting
	{
		/// <summary>
		/// 讀取配置的遠程網址
		/// </summary>
		public static string[] WEBURL = {
			"http://www.hilineage.com/gamelogin/",
			"http://www1.hilineage.com/gamelogin/",
            "http://www2.hilineage.com/gamelogin/",
            "http://www3.hilineage.com/gamelogin/",
            "http://www4.hilineage.com/gamelogin/",
            "http://localhost/"
        };
		
		private static string _website;
		
		public static string WebUrl
		{
			get
            {
                return _website;
            }
            set
            {
                _website = value;
            }
		}
		
		/// <summary>
		/// 讀取更新信息
		/// </summary>
		public static void readVersionInfo()
		{
			
		}
		
		public Setting()
		{
		}
	}
}

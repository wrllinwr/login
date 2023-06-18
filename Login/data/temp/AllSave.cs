using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.plugin;

namespace SFL.data.temp
{
	public class AllSave
	{		
		public static ArrayList USERLIST = new ArrayList();
	
		/// <summary>
		/// 登入的人物
		/// </summary>
		public static UserPc USER;
		
		/// <summary>
		/// 加速檢測機制
		/// </summary>
		public static SpeedCheck MOVESPEED = new SpeedCheck();
	
	}
}

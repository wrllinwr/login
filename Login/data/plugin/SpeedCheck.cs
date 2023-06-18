using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
// using System.Threading;
using System.Timers;
using System.Windows.Forms;
using SFL;
using SFL.data.temp;
using SFL.data.plugin;

namespace SFL.data.plugin
{	
	/// <summary>
	/// 加速器探測
	/// </summary>
	///
	public class SpeedCheck
	{	
		public enum ACT_TYPE
		{
			[Description("移動")]
			MOVE, // 移動
			[Description("攻擊")]
			ATTACK, // 攻擊
			[Description("有向施法")]
			SPELL_DIR, // 有向施法
			[Description("無向施法")]
			SPELL_NODIR // 無向施法
		}
		
		private int _injusticeCount;
	
		private int _justiceCount;
			
		private const int INJUSTICE_COUNT_LIMIT = 12; // 允許違規次數
			
		private const int JUSTICE_COUNT_LIMIT = 4; // 最少正常次數
			
		public const double CHECK_STRICTNESS = 1.02d; // 允許加速範圍
		
		private IDictionary<ACT_TYPE, Int64> _actTimers; // 封包時間記錄
		
		private IDictionary<ACT_TYPE, Int64> _checkTimers; // 封包時間記錄
			
		public const int R_OK = 0; // 檢測結果(正常)
	
		public const int R_DETECTED = 1; // 檢測結果(不正常)
	
		public const int R_DISCONNECTED = 2; // 檢測結果(達到臨界)
	
		public SpeedCheck()
		{
			this._injusticeCount = 0;
			this._justiceCount = 0;
			this._actTimers = new Dictionary<ACT_TYPE, Int64>();
			this._checkTimers = new Dictionary<ACT_TYPE, Int64>();
			this._injusticeCount = 0;
			this._justiceCount = 0;

			long now = (long) (DateTime.Now.Subtract(DateTime.Parse("1984-7-2")).TotalMilliseconds);

			foreach (SpeedCheck.ACT_TYPE  each  in  
			         (SpeedCheck.ACT_TYPE[]) Enum.GetValues(typeof(SpeedCheck.ACT_TYPE)))
			{
				if (this._actTimers.ContainsKey(each))
				{
					this._actTimers.Remove(each);
				}
				this._actTimers.Add(each, now);
				
				if (this._checkTimers.ContainsKey(each))
				{
					this._checkTimers.Remove(each);
				}
				this._checkTimers.Add(each, now);
			}
		}
	
		/// <summary>
		/// 移動與攻擊
		/// </summary>
		///
		/// <param name="type"></param>
		/// <returns></returns>
		public int CheckInterval(SpeedCheck.ACT_TYPE type)
		{
			int result = R_OK; // 結果初始化
			long now = (long) (DateTime.Now.Subtract(DateTime.Parse("1984-7-2")).TotalMilliseconds);
			long interval = now - this._actTimers[type]; // 獲取間隔時間			
			int rightInterval = this.GetRightInterval(type); // 獲取正確的間隔時間
	
			interval = (long) (interval * CHECK_STRICTNESS);
	
			if ((0 < interval) && (interval < rightInterval))
			{
				//System.out.println("違規:產生/允許:(" + type + ")"+interval + "/" + rightInterval);
				this._injusticeCount ++;
				this._justiceCount = 0;
	
				if (this._injusticeCount >= INJUSTICE_COUNT_LIMIT)
				{
					this.DoDisconnect();
				    return R_DISCONNECTED;
				}
				
				result = R_DETECTED;
	
			}
			else if (interval >= rightInterval)
			{
				//System.out.println("目前產生速率/允許最小速率:(" + type + ")"+interval + "/" + rightInterval);
				this._justiceCount ++;
	
				if (this._justiceCount >= JUSTICE_COUNT_LIMIT)
				{
					this._injusticeCount = 0;
					this._justiceCount = 0;
				}
			}
			
			if (this._actTimers.ContainsKey(type))
			{
				this._actTimers.Remove(type);
			}
			this._actTimers.Add(type, now);
			
			return result;
		}
	
		private void DoDisconnect()
		{
		    System.Timers.Timer quitTimer = new System.Timers.Timer(300);
			quitTimer.Elapsed += new ElapsedEventHandler(timers_Elapsed);
			quitTimer.AutoReset = true;
			quitTimer.Enabled = true;
			quitTimer.Start();
			
			MessageBox.Show("由於使用加速器被強行切斷連接！", "天堂",   
                    MessageBoxButtons.OK, MessageBoxIcon.Error,   
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly); 
            
            this._injusticeCount = 0;
			this._justiceCount = 0;
		}
	
		private static void timers_Elapsed(object sender, ElapsedEventArgs e)
		{
			Console.WriteLine("SpeedChecks.timers_elapsed处调用KillGame");
			Program.GetForm1().KillGame();
		}
		       
		private int GetRightInterval(SpeedCheck.ACT_TYPE  type)
		{
			int interval;
			switch (type)
			{
			case ACT_TYPE.ATTACK:
				interval = SprData.GetInstance().GetAttackSpeed(
						AllSave.USER.GFXID, AllSave.USER.WEAPONTYPE + 1);
				break;
	
			case ACT_TYPE.MOVE:
				interval = SprData.GetInstance().GetMoveSpeed(
						AllSave.USER.GFXID, AllSave.USER.WEAPONTYPE);
				break;
	
			case ACT_TYPE.SPELL_DIR:
				interval = SprData.GetInstance().GetDirSpellSpeed(
						AllSave.USER.GFXID);
				break;
	
			case ACT_TYPE.SPELL_NODIR:
				interval = SprData.GetInstance().GetNodirSpellSpeed(
						AllSave.USER.GFXID);
				break;
	
			default:
				return 0;
			}
			
			/*
			 * 0:你的情緒回復到正常。(解除 )
			 * 1:從身體的深處感到熱血沸騰。(第一階段勇水)
			 * 3:身體內深刻的感覺到充滿了森林的活力。(精靈餅乾)
			 * 4:風之疾走 / 神聖疾走 / 行走加速 / 生命之樹果實效果
			 * 5:從身體的深處感到熱血沸騰。(第二階段勇水)
			 * 6:引發龍之血暴發出來了。
			 */
			if (AllSave.USER.IsHaste())
			{
				//System.out.println("一段加速");
				interval = (int) (interval * 0.755d); // 0.755
			}
	
			if (type.Equals(SpeedCheck.ACT_TYPE.MOVE) && AllSave.USER.IsFastMovable())
			{
				//System.out.println("生命之樹果實效果");
				interval = (int) (interval * 0.665d); // 0.665
			}
	
			if (type.Equals(SpeedCheck.ACT_TYPE.ATTACK) && AllSave.USER.IsFastAttackable())
			{
				//System.out.println("血之渴望效果");
				interval = (int) (interval * 0.775d); // 0.775
			}
	
			if (AllSave.USER.IsBrave())
			{
				//System.out.println("勇敢藥水效果");
				interval = (int) (interval * 0.755d); // 0.755
			}
	
			if (AllSave.USER.IsElfBrave())
			{
				//System.out.println("精靈餅乾效果");
				interval = (int) (interval * 0.855d); // 0.855
			}
	
			if (type.Equals(SpeedCheck.ACT_TYPE.ATTACK) && AllSave.USER.IsElfBrave())
			{
				//System.out.println("精靈餅乾效果");
				interval = (int) (interval * 0.9d); // 0.9
			}
	
			if (AllSave.USER.IsPowerBrave())
			{
				//System.out.println("強化勇氣的藥水效果");
				interval = (int) (interval * 0.375d); // 0.375
			}
			
			if (AllSave.USER.IsSuperBrave())
			{
				//System.out.println("三段加速的藥水效果");
				interval = (int) (interval * 0.875d); // 0.870
			}
	
			return interval;
		}
	}
}

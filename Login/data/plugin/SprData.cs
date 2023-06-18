using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using SFL.data.plugin;
using SFL.Properties;

namespace SFL.data.plugin
{
	public class SprData
	{	
		private class Spr
		{
			public IDictionary<Int32, Int32> moveSpeed;	
			public IDictionary<Int32, Int32> attackSpeed;
	
			public int nodirSpellSpeed;	
			public int dirSpellSpeed;
			
			public Spr()
			{
				this.moveSpeed = new Dictionary<Int32, Int32>();
				this.attackSpeed = new Dictionary<Int32, Int32>();
				this.nodirSpellSpeed = 1200;
				this.dirSpellSpeed = 1200;
			}
		}
	
		private static IDictionary<Int32, Spr> _dataMap = new Dictionary<Int32, Spr>();
	
		private static SprData _instance;
	
		public static SprData GetInstance()
		{
			if (_instance == null)
			{
				_instance = new SprData();
			}
			
			return _instance;
		}
	
		/// <summary>
		/// spr_action載入
		/// </summary>
		///
		public void Load()
		{
			SprData.Spr spr = null;
			
			try
			{
				string md5FromIni = Program.GetSprMd5();
				byte[] md5bytes = Convert.FromBase64String(md5FromIni);
                md5FromIni = Encoding.UTF8.GetString(md5bytes, 0, md5bytes.Length);
                    
				string locFile = Directory.GetCurrentDirectory() + "\\" + "npklogin3.sys";
				// 檢查文件是否存在
				if (!File.Exists(locFile))
				{
					MessageBox.Show("配置文件(3)缺失，請重新下載登陸器！", "天堂");
					throw(new Exception());
				}
				
				// 校驗MD5值
				if (!Util.GetMD5HashFromFile(locFile).Equals(md5FromIni))
				{
					MessageBox.Show("配置文件(3)讀取錯誤，請重新下載登陸器！", "天堂");
					throw(new Exception());
				}
				
				StreamReader oriFileSr = File.OpenText(locFile); // 原始文件
	
				int i = 0;
				String st;
				// 開始讀取資料
				while ((st = oriFileSr.ReadLine()) != null)
				{
					// 解密作業
			        byte[] stbytes = Convert.FromBase64String(st);
                    st = Encoding.UTF8.GetString(stbytes, 0, stbytes.Length);
                
					// 此段落為註解
					if (st.StartsWith("#"))
					{
						continue;
					}
					
					i ++;
					StringTokenizer sprSt = new StringTokenizer(st, ","); //  以逗號為分隔符
					int key = Int32.Parse(sprSt.NextToken());
					int actid = Int32.Parse(sprSt.NextToken());
					int frameCount = Int32.Parse(sprSt.NextToken());
					int frameRate = Int32.Parse(sprSt.NextToken());
	
					if (!_dataMap.ContainsKey(key))
					{
						spr = new SprData.Spr();
						_dataMap.Add(key, spr);
					}
					else
					{
						spr = _dataMap[key];
					}
	
					int speed = this.CalcActionSpeed(frameCount, frameRate);
	
					switch (actid)
					{
					case ActionCode.ACTION_Walk:
					case ActionCode.ACTION_SwordWalk:
					case ActionCode.ACTION_AxeWalk:
					case ActionCode.ACTION_BowWalk:
					case ActionCode.ACTION_SpearWalk:
					case ActionCode.ACTION_StaffWalk:
					case ActionCode.ACTION_DaggerWalk:
					case ActionCode.ACTION_TwoHandSwordWalk:
					case ActionCode.ACTION_EdoryuWalk:
					case ActionCode.ACTION_ClawWalk:
					case ActionCode.ACTION_ThrowingKnifeWalk:
						if (spr.moveSpeed.ContainsKey(actid))
						{
							spr.moveSpeed.Remove(actid);
						}
						spr.moveSpeed.Add(actid, speed);
						break;
	
					case ActionCode.ACTION_SkillAttack:
						spr.dirSpellSpeed = speed;
						break;
	
					case ActionCode.ACTION_SkillBuff:
						spr.nodirSpellSpeed = speed;
						break;
	
					case ActionCode.ACTION_Attack:
					case ActionCode.ACTION_SwordAttack:
					case ActionCode.ACTION_AxeAttack:
					case ActionCode.ACTION_BowAttack:
					case ActionCode.ACTION_SpearAttack:
					case ActionCode.ACTION_StaffAttack:
					case ActionCode.ACTION_DaggerAttack:
					case ActionCode.ACTION_TwoHandSwordAttack:
					case ActionCode.ACTION_EdoryuAttack:
					case ActionCode.ACTION_ClawAttack:
					case ActionCode.ACTION_ThrowingKnifeAttack:
						if (spr.attackSpeed.ContainsKey(actid))
						{
							spr.attackSpeed.Remove(actid);
						}
						spr.attackSpeed.Add(actid, speed);
						break;
	
					default:
						break;
					}
				}
				
				oriFileSr.Close();
			}
			catch (Exception e)
			{
				throw e;
			}
		}
		        
		/// <summary>
		/// 計算速度影格
		/// </summary>
		///
		private int CalcActionSpeed(int frameCount, int frameRate)
		{
			return (int) (frameCount * 40 * (24D / frameRate));
		}
	
		/// <summary>
		/// 傳回指定外型武器攻擊速度
		/// </summary>
		///
		/// <param name="sprid">外型編號</param>
		/// <param name="actid">- 武器類型</param>
		/// <returns>速度</returns>
		public int GetAttackSpeed(int sprid, int actid)
		{
			if (_dataMap.ContainsKey(sprid))
			{
				if (_dataMap[sprid].attackSpeed.ContainsKey(actid))
				{
					return _dataMap[sprid].attackSpeed[actid];
				} 
				else if (actid == ActionCode.ACTION_Attack)
				{
					return 0;
				} 
				else
				{
					return _dataMap[sprid].attackSpeed[ActionCode.ACTION_Attack];
				}
			}
			
			return 0;
		}
	
		/// <summary>
		/// 傳回指定外型移動速度
		/// </summary>
		///
		/// <param name="sprid"></param>
		/// <param name="actid"></param>
		/// <returns></returns>
		public int GetMoveSpeed(int sprid, int actid)
		{
			if (_dataMap.ContainsKey(sprid))
			{
				if (_dataMap[sprid].moveSpeed.ContainsKey(actid))
				{
					return _dataMap[sprid].moveSpeed[actid];
				}
				else if (actid == ActionCode.ACTION_Walk)
				{
					return 0;
				}
				else
				{
					return _dataMap[sprid].moveSpeed[ActionCode.ACTION_Walk];
				}
			}
			
			return 0;
		}
	
		public int GetDirSpellSpeed(int sprid)
		{
			if (_dataMap.ContainsKey(sprid))
			{
				
				return _dataMap[sprid].dirSpellSpeed;
			}
			
			return 0;
		}
	
		public int GetNodirSpellSpeed(int sprid)
		{
			if (_dataMap.ContainsKey(sprid))
			{
				return _dataMap[sprid].nodirSpellSpeed;
			}
			
			return 0;
		}
	}
}

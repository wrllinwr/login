using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.temp
{
	public class UserPc
	{	
		public int OBJID; // 世界ID
		public String NAME; // 角色名字
		public int LIGHT; // 亮度	
		public int MOVESPEED; // 移動速度
		public int BRAVESPEED; // 攻擊速度
		public int LIQUORSTATE; // 喝酒或者三段加速
	
		public int GFXID; // 參考值:外型編號
		public int X; // 參考值:X座標
		public int Y; // 參考值:Y座標
		public int HEADING; // 參考值:面向
		public int WEAPONTYPE; // 參考值:武器類型
		
		public UserPc()
		{
			this.OBJID = 0;
			this.NAME = null;
			this.LIGHT = 0;
			this.MOVESPEED = 0;
			this.BRAVESPEED = 0;
			this.LIQUORSTATE = 0;
			this.GFXID = 0;
			this.X = 0;
			this.Y = 0;
			this.HEADING = 0;
			this.WEAPONTYPE = 0;
		}

		/// <summary>
		/// 移動加速(一段加速)
		/// </summary>
		///
		/// <returns></returns>
		public bool IsHaste()
		{
			switch (this.MOVESPEED)
			{
			case 1:
				return true;
			}
			
			return false;
		}
	
		/// <summary>
		/// 神聖疾走效果
		/// 行走加速效果
		/// 風之疾走效果
		/// 生命之樹果實效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsFastMovable()
		{
			switch (this.BRAVESPEED)
			{
			case 4:
				return true;
			}
			
			return false;
		}
	
		/// <summary>
		/// 勇敢藥水效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsBrave()
		{
			switch (this.BRAVESPEED)
			{
			case 1:
				return true;
			}
			
			return false;
		}
	
		/// <summary>
		/// 強化勇氣的藥水效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsPowerBrave()
		{
			switch (this.BRAVESPEED)
			{
			case 5:
				return true;
			}
			
			return false;
		}
	
		/// <summary>
		/// 血之渴望效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsFastAttackable()
		{
			switch (this.BRAVESPEED)
			{
			case 6:
				return true;
			}
			
			return false;
		}
	
		/// <summary>
		/// 精靈餅乾效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsElfBrave()
		{
			switch (this.BRAVESPEED)
			{
			case 3:
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// 三段加速效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsSuperBrave()
		{
			switch (this.LIQUORSTATE)
			{
			case 8:
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// 醉酒效果
		/// </summary>
		///
		/// <returns></returns>
		public bool IsDrunk()
		{
			switch (this.LIQUORSTATE)
			{
			case 1:
				return true;
			}
			
			return false;
		}
	}
}

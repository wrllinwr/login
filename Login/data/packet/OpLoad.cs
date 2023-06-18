using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.packet.client;
using SFL.data.packet.server;

namespace SFL.data.packet
{	
	public class OpLoad 
	{
		// 客戶端
		
		/// <summary>
		/// (客戶端)要求角色攻擊
		/// </summary>
		///
		protected static internal int C_OPCODE_ATTACK;
	
		/// <summary>
		/// (客戶端)要求角色攻擊(遠距離)
		/// </summary>
		///
		protected static internal int C_OPCODE_ARROWATTACK;

		/// <summary>
		/// (客戶端)要求角色使用魔法
		/// </summary>
		///
		protected static internal int C_OPCODE_USESKILL;
		
		/// <summary>
		/// (客戶端)要求角色移動
		/// </summary>
		///
		protected static internal int C_OPCODE_MOVECHAR;
		
		/// <summary>
		/// (客戶端)要求進入遊戲
		/// </summary>
		///
		protected static internal int C_OPCODE_LOGINTOSERVER;
		
		/// <summary>
		/// (客戶端)要求切換角色
		/// </summary>
		///
		protected static internal int C_OPCODE_CHANGECHAR;
	
		// 服務端
	
		/// <summary>
		/// (服務端)伺服器訊息(行數/行數,附加字串)
		/// </summary>
		///
		protected static internal int S_OPCODE_SERVERMSG;
		
		/// <summary>
		/// (服務端)魔法效果 三段加速纇
		/// </summary>
		///
		protected static internal int S_OPCODE_LIQUOR;
	
		/// <summary>
		/// (服務端)魔法效果 勇敢藥水纇
		/// </summary>
		///
		protected static internal int S_OPCODE_SKILLBRAVE;
	
		/// <summary>
		/// (服務端)魔法效果 加速纇
		/// </summary>
		///
		protected static internal int S_OPCODE_SKILLHASTE;
	
		/// <summary>
		/// (服務端)物件動作種類(長時間)
		/// </summary>
		///
		protected static internal int S_OPCODE_CHARVISUALUPDATE;
	
		/// <summary>
		/// (服務端)物件移動
		/// </summary>
		///
		protected static internal int S_OPCODE_MOVEOBJECT;
	
		/// <summary>
		/// (服務端)物件封包
		/// </summary>
		///
		protected static internal int S_OPCODE_CHARPACK;
	
		/// <summary>
		/// (服務端)角色列表資訊
		/// </summary>
		///
		protected static internal int S_OPCODE_CHARLIST;
	
		/// <summary>
		/// (服務端)角色資訊
		/// </summary>
		///
		protected static internal int S_OPCODE_OWNCHARSTATUS;
	
		/// <summary>
		/// (服務端)更新物件外型
		/// </summary>
		///
		protected static internal int S_OPCODE_POLY;
	
		/// <summary>
		/// (服務端)立即中斷連線
		/// </summary>
		///
		protected static internal int S_OPCODE_DISCONNECT;
	
		/// <summary>
		/// (服務端)創造角色(新創)
		/// </summary>
		///
		protected static internal int S_OPCODE_NEWCHARPACK;
	
		public static readonly IDictionary<Int32, BasePacket> COPLIST = new Dictionary<Int32, BasePacket>();
		public static readonly IDictionary<Int32, BasePacket> SOPLIST = new Dictionary<Int32, BasePacket>();
	
		public static void Load(long seed)
		{
			// Lin.ver 12011702 351C_TW
	        // 客戶端封包賦值
		    C_OPCODE_ATTACK = 129;
			C_OPCODE_ARROWATTACK = 20;
			C_OPCODE_USESKILL = 71;
			C_OPCODE_MOVECHAR = 24;
			C_OPCODE_LOGINTOSERVER = 5;
			C_OPCODE_CHANGECHAR = 111;
	        // 服務端封包賦值
	        S_OPCODE_LIQUOR = 2;
			S_OPCODE_SKILLBRAVE = 96;
			S_OPCODE_SKILLHASTE = 125;
			S_OPCODE_CHARVISUALUPDATE = 17;
			S_OPCODE_DISCONNECT = 113;
			S_OPCODE_MOVEOBJECT = 0;
			S_OPCODE_NEWCHARPACK = 10;
			S_OPCODE_CHARPACK = 97;
			S_OPCODE_CHARLIST = 37;
			S_OPCODE_OWNCHARSTATUS = 30;
			S_OPCODE_POLY = 31;
			S_OPCODE_SERVERMSG = 63;
	
			// 客戶端加載封包編號
			COPLIST.Add(C_OPCODE_ATTACK, new C_Attack());
			COPLIST.Add(C_OPCODE_ARROWATTACK, new C_AttackBow());
			COPLIST.Add(C_OPCODE_USESKILL, new C_UseSkill());
			COPLIST.Add(C_OPCODE_MOVECHAR, new C_MoveChar());
			COPLIST.Add(C_OPCODE_LOGINTOSERVER, new C_LoginToServer());
			COPLIST.Add(C_OPCODE_CHANGECHAR, new C_ChangeChar());
	        // 服務端加載封包編號	        
	        SOPLIST.Add(S_OPCODE_LIQUOR, new S_Liquor());
			SOPLIST.Add(S_OPCODE_SKILLBRAVE, new S_SkillBrave());
			SOPLIST.Add(S_OPCODE_SKILLHASTE, new S_SkillHaste());
			SOPLIST.Add(S_OPCODE_CHARVISUALUPDATE, new S_CharVisualUpdate());
			SOPLIST.Add(S_OPCODE_MOVEOBJECT, new S_MoveCharPacket());
			SOPLIST.Add(S_OPCODE_POLY, new S_ChangeShape());
			SOPLIST.Add(S_OPCODE_SERVERMSG, new S_ServerMessage());
			SOPLIST.Add(S_OPCODE_CHARPACK, new S_OwnCharPacks());
			SOPLIST.Add(S_OPCODE_OWNCHARSTATUS, new S_OwnCharStatus());	
			SOPLIST.Add(S_OPCODE_DISCONNECT, new S_Disconnect());
			SOPLIST.Add(S_OPCODE_NEWCHARPACK, new S_NewCharPacket());
			SOPLIST.Add(S_OPCODE_CHARLIST, new S_CharList());
		}
	}
}

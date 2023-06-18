using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.plugin;

namespace SFL.data.packet
{	
	public abstract class BasePacket
	{
	
		private byte[] _decrypt; // 初始封包
	
		private int _off; // 正在讀取的位數
	
		public abstract bool Execute(byte[] abyte0); // 具體的執行方法，待繼承
	
		/// <summary>
		/// 載入封包
		/// </summary>
		///
		/// <param name="abyte0">byte[]封包數據</param>
		protected internal void Read(byte[] abyte0)
		{
			this._decrypt = abyte0;
			this._off = 1;
		}

		/// <summary>
		/// 讀取8位二進制數據(1次16進制 FF)
		/// </summary>
		protected internal int ReadC()
		{
			int i = this._decrypt[this._off++] & 0xff;
			return i;
		}
	
		/// <summary>
		/// 讀取16位二進制數據(2次16進制 FF FF)
		/// </summary>
		protected internal int ReadH()
		{
			int i = this._decrypt[this._off++] & 0xff;
			i |= this._decrypt[this._off++] << 8 & 0xff00;
			return i;
		}
	
		/// <summary>
		/// 讀取16位二進制數據(2次16進制 FF FF FF)
		/// </summary>
		protected internal int ReadCH()
		{
			int i = this._decrypt[this._off++] & 0xff;
			i |= this._decrypt[this._off++] << 8 & 0xff00;
			i |= this._decrypt[this._off++] << 16 & 0xff0000;
			return i;
		}
	
		/// <summary>
		/// 讀取32位(int)二進制數據(4次16進制 FF FF FF FF)
		/// </summary>
		protected internal int ReadD()
		{
			int i = this._decrypt[this._off++] & 0xff;
			i |= this._decrypt[this._off++] << 8 & 0xff00;
			i |= this._decrypt[this._off++] << 16 & 0xff0000;
			i |= this._decrypt[this._off++] << 24 & -16777216;
			return i;
		}
		
		/// <summary>
		/// 讀取64位(double)二進制數據(8次16進制 FF FF FF FF FF FF FF FF)
		/// </summary>
		protected internal double ReadF()
		{
			long l = this._decrypt[this._off++] & 0xff;
			l |= (long) this._decrypt[this._off++] << 8 & 0xff00;
			l |= (long) this._decrypt[this._off++] << 16 & 0xff0000;
			l |= (long) this._decrypt[this._off++] << 24 & -16777216;
			l |= (long) this._decrypt[this._off++] << 32 & 0xff00000000L;
			l |= (long) this._decrypt[this._off++] << 40 & 0xff0000000000L;
			l |= (long) this._decrypt[this._off++] << 48 & 0xff000000000000L;
			l |= (long) this._decrypt[this._off++] << 56 & -72057594037927936;
			return BitConverter.Int64BitsToDouble(l);
		}
	
		/// <summary>
		/// 讀取字符串數據 疑點
		/// </summary>
		protected internal String ReadS()
		{
			String s = null;
			try {
				byte[] result = new byte[this._decrypt.Length - this._off];
				System.Array.Copy(
					(Array)(this._decrypt), this._off, (Array)(result), 0, (this._decrypt.Length - this._off));
				s = NewString(result);
				s = s.Substring(0, (s.IndexOf('\0') - 0));
				this._off += GetBytes(s).Length + 1;
			}
			catch {}
			
			return s;
		}
	
		/// <summary>
		/// 讀取byte[]數據
		/// </summary>
		protected internal byte[] ReadByte()
		{
			byte[] result = new byte[this._decrypt.Length - this._off];
			try {
				System.Array.Copy(
					(Array)(this._decrypt), this._off, (Array)(result), 0, (this._decrypt.Length - this._off));
				this._off = this._decrypt.Length;
			}
			catch {}
			
			return result;
		}
		
		private static string NewString(byte[] p)
		{
			string text = "";
			for (int i = 0; i < p.Length; i++)
			{
				text += (char)p[i];
			}
			
			return text;
		}
		
		private static byte[] GetBytes(string stingMessage)
		{
			byte[] array = new byte[stingMessage.Length];
			for (int i = 0; i < stingMessage.Length; i++)
			{
				array[i] = (byte)stingMessage[i];
			}
			
			return array;
		}
		
	    protected void over()
	    {
	        this._decrypt = null;
			this._off = 0;
	    }
	}
}

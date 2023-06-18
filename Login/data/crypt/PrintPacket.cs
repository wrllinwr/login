using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SFL.data.crypt
{
	/// <summary>
	/// 封包資料解密(封包監控用)
	/// </summary>
	///
	public class PacketPrint
	{
		private static int CharacterMIN_RADIX = 2;
		private static int CharacterMAX_RADIX = 36;
		private static PacketPrint _data;
	
		public static PacketPrint Get()
		{
			if (_data == null)
			{
				_data = new PacketPrint();
			}
			
			return _data;
		}
	
		/// <summary>
		/// <font color=#0000ff>印出封包</font>
		/// 目的:<BR>
		/// 用於檢查客戶端傳出的封包資料<BR>
		/// </summary>
		///
		/// <param name="data"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public String PrintData(byte[] data, int len)
		{	
			StringBuilder result = new StringBuilder();
	
			int counter = 0;
	
			for (int i = 0; i < len; i++)
			{
	
				if (counter % 16 == 0)
				{
					result.Append(this.FillHex(i, 4) + ": ");
				}
	
				result.Append(this.FillHex(data[i] & 0xff, 2) + " ");
				counter++;
	
				if (counter == 16)
				{
					result.Append("   ");
	
					int charpoint = i - 15;
					for (int a = 0; a < 16; a++)
					{
						int t1 = data[charpoint++];
	
						if ((t1 > 0x1f) && (t1 < 0x80))
						{
							result.Append((char) t1);
						}
						else
						{
							result.Append('.');
						}
					}
	
					result.Append("\n");
					counter = 0;
				}
			}
	
			int rest = data.Length % 16;
	
			if (rest > 0)
			{	
				for (int i_0 = 0; i_0 < 17 - rest; i_0++)
				{
					result.Append("   ");
				}
	
				int charpoint_1 = data.Length - rest;
	
				for (int a_2 = 0; a_2 < rest; a_2++)
				{	
					int t1_3 = data[charpoint_1++];
	
					if ((t1_3 > 0x1f) && (t1_3 < 0x80))
					{
						result.Append((char) t1_3);
					}
					else
					{
						result.Append('.');
					}
				}
	
				result.Append("\n");
			}
	
			return result.ToString();
		}
	
		/// <summary>
		/// <font color=#0000ff>將數字轉成 16 進位</font>
		/// </summary>
		///
		/// <param name="data"></param>
		/// <param name="digits"></param>
		/// <returns></returns>
		private String FillHex(int data, int digits)
		{	
			String number = ToString(data,16);
	
			for (int i = number.Length; i < digits; i++)
			{
				number = "0" + number;
			}
	
			return number;
		}
	
		private static string ToString(int i, int radix)
		{
			if (radix < CharacterMIN_RADIX || radix > CharacterMAX_RADIX)
			{
				radix = 10;
			}
			string result;
			if (i == 0)
			{
				result = "0";
			}
			else
			{
				int num = 2;
				int num2 = i;
				if (i >= 0)
				{
					num = 1;
					num2 = -i;
				}
				while ((i /= radix) != 0)
				{
					num++;
				}
				char[] array = new char[num];
				if (i < 0)
				{
					array[0] = '-';
				}
				do
				{
					int num3 = -(num2 % radix);
					if (num3 > 9)
					{
						num3 = num3 - 10 + 97;
					}
					else
					{
						num3 += 48;
					}
					array[--num] = (char)num3;
				}
				while ((num2 /= radix) != 0);
				result = new string(array, 0, array.Length);
			}
			return result;
		}
		
	}
}

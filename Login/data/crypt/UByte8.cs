using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.crypt
{	
	public class UByte8
	{
		/// <summary>
		/// Converts a 32 bit unsigned/signed long array to a 8 bit unsigned byte
		/// array.
		/// </summary>
		///
		/// <param name="buff">the array to convert</param>
		/// <returns>byte[] an 8 bit unsigned byte array</returns>
		public static byte[] FromArray(long[] buff)
		{
			byte[] byteBuff = new byte[buff.Length * 4];
	
			for (int i = 0; i < buff.Length; ++i)
			{
				byteBuff[(i * 4) + 0] = (byte) (buff[i] & 0xFF);
				byteBuff[(i * 4) + 1] = (byte) ((buff[i] >> 8) & 0xFF);
				byteBuff[(i * 4) + 2] = (byte) ((buff[i] >> 16) & 0xFF);
				byteBuff[(i * 4) + 3] = (byte) ((buff[i] >> 24) & 0xFF);
			}
	
			return byteBuff;
		}
	
		/// <summary>
		/// Converts an 8 bit unsigned char array to an 8 bit unsigned byte array.
		/// </summary>
		///
		/// <param name="buff">the array to convert</param>
		/// <returns>byte[] an 8 bit unsigned byte array</returns>
		public static byte[] FromArray(char[] buff)
		{
			byte[] byteBuff = new byte[buff.Length];
	
			for (int i = 0; i < buff.Length; ++i)
			{
				byteBuff[i] = (byte) (buff[i] & 0xFF);
			}
	
			return byteBuff;
		}
	
		/// <summary>
		/// Converts an 8 bit unsigned char to an 8 bit unsigned byte.
		/// </summary>
		///
		/// <param name="c">the char value to convert</param>
		/// <returns>byte an 8 bit unsigned byte</returns>
		public static byte FromUChar8(char c)
		{
			return (byte) (c & 0xFF);
		}
	
		/// <summary>
		/// Converts a 32 bit unsigned long to an 8 bit unsigned byte.
		/// </summary>
		///
		/// <param name="l">the long value to convert</param>
		/// <returns>byte an 8 bit unsigned char</returns>
		public static byte[] FromULong32(long l)
		{
			byte[] byteBuff = new byte[4];
	
			byteBuff[0] = (byte) (l & 0xFF);
			byteBuff[1] = (byte) ((l >> 8) & 0xFF);
			byteBuff[2] = (byte) ((l >> 16) & 0xFF);
			byteBuff[3] = (byte) ((l >> 24) & 0xFF);
	
			return byteBuff;
		}
		
	}
}

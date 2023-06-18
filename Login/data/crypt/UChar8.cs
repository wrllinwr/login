using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.crypt
{
	public class UChar8
	{
		/// <summary>
		/// Converts a 32 bit unsigned/signed long array to a 8 bit unsigned char
		/// array.
		/// </summary>
		///
		/// <param name="buff">the array to convert</param>
		/// <returns>char[] an 8 bit unsigned char array</returns>
		public static char[] FromArray(long[] buff)
		{
			char[] charBuff = new char[buff.Length * 4];
	
			for (int i = 0; i < buff.Length; ++i)
			{
				charBuff[(i * 4) + 0] = (char) (buff[i] & 0xFF);
				charBuff[(i * 4) + 1] = (char) ((buff[i] >> 8) & 0xFF);
				charBuff[(i * 4) + 2] = (char) ((buff[i] >> 16) & 0xFF);
				charBuff[(i * 4) + 3] = (char) ((buff[i] >> 24) & 0xFF);
			}
	
			return charBuff;
		}
	
		/// <summary>
		/// Converts an 8 bit unsigned byte array to an 8 bit unsigned char array.
		/// </summary>
		///
		/// <param name="buff">the array to convert</param>
		/// <returns>char[] an 8 bit unsigned char array</returns>
		public static char[] FromArray(byte[] buff)
		{
			char[] charBuff = new char[buff.Length];
	
			for (int i = 0; i < buff.Length; ++i)
			{
				charBuff[i] = (char) (buff[i] & 0xFF);
			}
	
			return charBuff;
		}
	
		/// <summary>
		/// Converts an 8 bit unsigned byte array to an 8 bit unsigned char array.
		/// </summary>
		///
		/// <param name="buff">the array to convert length the array size</param>
		/// <returns>char[] an 8 bit unsigned char array</returns>
		public static char[] FromArray(byte[] buff, int length)
		{
			char[] charBuff = new char[length];
	
			for (int i = 0; i < length; ++i)
			{
				charBuff[i] = (char) (buff[i] & 0xFF);
			}
	
			return charBuff;
		}
	
		/// <summary>
		/// Converts an 8 bit unsigned byte to an 8 bit unsigned char.
		/// </summary>
		///
		/// <param name="b">the byte value to convert</param>
		/// <returns>char an 8 bit unsigned char</returns>
		public static char FromUByte8(byte b)
		{
			return (char) (b & 0xFF);
		}
	
		/// <summary>
		/// Converts a 32 bit unsigned long to an 8 bit unsigned char.
		/// </summary>
		///
		/// <param name="l">the long value to convert</param>
		/// <returns>char an 8 bit unsigned char</returns>
		public static char[] FromULong32(long l)
		{
			char[] charBuff = new char[4];
	
			charBuff[0] = (char) (l & 0xFF);
			charBuff[1] = (char) ((l >> 8) & 0xFF);
			charBuff[2] = (char) ((l >> 16) & 0xFF);
			charBuff[3] = (char) ((l >> 24) & 0xFF);
	
			return charBuff;
		}
	}
}

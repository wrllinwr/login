using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SFL.data.crypt
{
	/// <summary>
	/// Handles 32 bit unsigned long conversions, due to the lack in the java core.
	/// </summary>
	///
	public class ULong32
	{
		public const long MAX_UNSIGNEDLONG_VALUE = 2147483647L;
	
		/// <summary>
		/// Converts a 8 bit char buffer to a 32 bit unsigned long.
		/// </summary>
		///
		/// <param name="buff">the buffer to convert</param>
		/// <returns>long unsigned long value stored as a long</returns>
		public static long FromArray(char[] buff)
		{
			return FromLong64(((buff[3] & 0xFF) << 24) | ((buff[2] & 0xFF) << 16)
					| ((buff[1] & 0xFF) << 8) | (buff[0] & 0xFF));
		}
	
		/// <summary>
		/// Converts a 8 bit byte buffer to a 32 bit unsigned long.
		/// </summary>
		///
		/// <param name="buff">the buffer to convert</param>
		/// <returns>long unsigned long value stored as a long</returns>
		public static long FromArray(byte[] buff)
		{
			return FromLong64(((buff[3] & 0xFF) << 24) | ((buff[2] & 0xFF) << 16)
					| ((buff[1] & 0xFF) << 8) | (buff[0] & 0xFF));
		}
	
		/// <summary>
		/// Converts a 64 bit, java's standard, long to a 32 bit unsigned long. Chops
		/// away the high 32 bits
		/// </summary>
		///
		/// <param name="l">the long value to convert</param>
		/// <returns>long unsigned long value stored as a long</returns>
		public static long FromLong64(long l)
		{
			return (((long) (((ulong) (l << 32)) >> 32)) & -1);
		}
	
		/// <summary>
		/// Converts a 32 bit, java's standard, int to a 32 bit unsigned long.
		/// </summary>
		///
		/// <param name="i">the int value to convert</param>
		/// <returns>long unsigned long value stored as a long</returns>
		public static long FromInt32(int i)
		{
			return (((long) (((ulong) ((long)i << 32)) >> 32)) & -1);
		}
	
		/// <summary>
		/// Adds two 32 bit unsigned/signed long values
		/// </summary>
		///
		/// <param name="l1">the addee</param>
		/// <param name="l2">to be added</param>
		/// <returns>long unsigned long value stored as a long</returns>
		public static long Add(long l1, long l2)
		{
			return FromInt32((int) l1 + (int) l2);
		}
	
		/// <summary>
		/// Subtracts two 32 bit unsigned/signed long values
		/// </summary>
		///
		/// <param name="l1">the subtractee</param>
		/// <param name="l2">to be subtracted</param>
		/// <returns>long unsigned long value stored as a long</returns>
		public static long Sub(long l1, long l2)
		{
			return FromInt32((int) l1 - (int) l2);
		}
	}
}

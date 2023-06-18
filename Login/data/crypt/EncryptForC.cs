using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SFL.data.crypt;
using SFL.data.crypt.exception;

namespace SFL.data.crypt
{
	/// <summary>
	/// 加密解密(客戶端用)
	/// </summary>
	public class EncryptForC
	{	
		private static LineageKeys currentKeys;
	
		public static void InitKeys(long seed)
		{
			currentKeys = new LineageKeys();	
			long[] key = { seed, 0x930FD7E2L };	
			LinBlowfish.GetSeeds(key);
	
			currentKeys.encodeKey[0] = currentKeys.decodeKey[0] = key[0];
			currentKeys.encodeKey[1] = currentKeys.decodeKey[1] = key[1];
		}
	
		/// <summary>
		/// 加密
		/// </summary>
		public static byte[] Encrypt(byte[] buf)
		{
			if (currentKeys == null)
			{
				throw new NoKeySelected();
			}
	
			long mask = ULong32.FromArray(buf);	
			_encrypt(buf);
	
			currentKeys.encodeKey[0] ^= mask;
			currentKeys.encodeKey[1] = ULong32.Add(currentKeys.encodeKey[1], 0x287EFFC3L);
	
			return buf;
		}
	
		/// <summary>
		/// 解密
		/// </summary>
		///
		public static byte[] Decrypt(byte[] buf, int length)
		{
			if (currentKeys == null)
			{
				throw new NoKeySelected();
			}
	
			_decrypt(buf, length);	
			long mask = ULong32.FromArray(buf);
	
			currentKeys.decodeKey[0] ^= mask;
			currentKeys.decodeKey[1] = ULong32.Add(currentKeys.decodeKey[1], 0x287EFFC3L);
	
			return buf;
		}
	
		private static byte[] _encrypt(byte[] buf)
		{
			int size = buf.Length;
			char[] ek = UChar8.FromArray(currentKeys.encodeKey);
	
			buf[0] = (byte) (buf[0] ^ ek[0]);
	
			for (int i = 1; i < size; i++) {
				buf[i] ^= (byte) (buf[i - 1] ^ ek[i & 7]);
			}
	
			buf[3] = (byte) (buf[3] ^ ek[2]);
			buf[2] = (byte) (buf[2] ^ buf[3] ^ ek[3]);
			buf[1] = (byte) (buf[1] ^ buf[2] ^ ek[4]);
			buf[0] = (byte) (buf[0] ^ buf[1] ^ ek[5]);
	
			return buf;
		}
	
		private static byte[] _decrypt(byte[] buf, int size)
		{
			char[] dk = UChar8.FromArray(currentKeys.decodeKey);
	
			byte b3 = buf[3];
			buf[3] = (byte) (buf[3] ^ dk[2]);
	
			byte b2 = buf[2];
			buf[2] ^= (byte) (b3 ^ dk[3]);
	
			byte b1 = buf[1];
			buf[1] ^= (byte) (b2 ^ dk[4]);
	
			byte k = (byte) (buf[0] ^ b1 ^ dk[5]);
			buf[0] = (byte) (k ^ dk[0]);
	
			for (int i = 1; i < size; i++)
			{
				byte t = buf[i];
				buf[i] ^= (byte) (dk[i & 7] ^ k);
				k = t;
			}
			
			return buf;
		}
	}
}

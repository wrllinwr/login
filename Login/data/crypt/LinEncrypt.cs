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
	/// Handler Encryption/Decryption of lineage packet data
	/// </summary>
	///
	public class LinEncrypt
	{	
		public static LineageKeys _initKeys;
	
		/// <summary>
		/// 設置加密解密模組
		/// </summary>
		///
		/// <param name="clientID"></param>
		/// <param name="seed"></param>
		/// <exception cref="ClientIdExistsException"></exception>
		public static void InitKeys(long seed)
		{
			LineageKeys keys = new LineageKeys();
	
			long[] key = { seed, 0x930FD7E2L };
	
			LinBlowfish.GetSeeds(key);
	
			keys.encodeKey[0] = keys.decodeKey[0] = key[0];
			keys.encodeKey[1] = keys.decodeKey[1] = key[1];
	
			_initKeys = keys;
		}
	
		/// <summary>
		/// 加密
		/// </summary>
		///
		/// <param name="buf">char[]</param>
		/// <param name="currentKeys">LineageKeys</param>
		/// <returns></returns>
		/// <exception cref="NoEncryptionKeysSelectedException"></exception>
		public static char[] Encrypt(char[] buf)
		{
			if (_initKeys == null)
			{
				throw new NoKeySelected();
			}
	
			long mask = ULong32.FromArray(buf);
	
			_encrypt(buf);
	
			_initKeys.encodeKey[0] ^= mask;
			_initKeys.encodeKey[1] = ULong32.Add(_initKeys.encodeKey[1], 0x287EFFC3L);
	
			return buf;
		}
	
		/// <summary>
		/// 加密處理
		/// </summary>
		///
		/// <param name="buf"></param>
		/// <param name="currentKeys"></param>
		/// <returns></returns>
		private static char[] _encrypt(char[] buf)
		{
			int size = buf.Length;
			char[] ek = UChar8.FromArray(_initKeys.encodeKey);
	
			buf[0] ^= ek[0];
	
			for (int i = 1; i < size; i++)
			{
				buf[i] ^= ((Char)(buf[i - 1] ^ ek[i & 7]));
			}
	
			buf[3] = (char) (buf[3] ^ ek[2]);
			buf[2] = (char) (buf[2] ^ buf[3] ^ ek[3]);
			buf[1] = (char) (buf[1] ^ buf[2] ^ ek[4]);
			buf[0] = (char) (buf[0] ^ buf[1] ^ ek[5]);
	
			return buf;
		}
	
		/// <summary>
		/// 加密
		/// </summary>
		///
		/// <param name="buf">byte[]</param>
		/// <param name="currentKeys">LineageKeys</param>
		/// <returns></returns>
		/// <exception cref="NoEncryptionKeysSelectedException"></exception>
		public static byte[] Encrypt(byte[] buf)
		{
			if (_initKeys == null)
			{
				throw new NoKeySelected();
			}
	
			long mask = ULong32.FromArray(buf);
	
			_encrypt(buf);
	
			_initKeys.encodeKey[0] ^= mask;
			_initKeys.encodeKey[1] = ULong32.Add(_initKeys.encodeKey[1], 0x287EFFC3L);
	
			return buf;
		}
	
		/// <summary>
		/// 加密處理
		/// </summary>
		///
		/// <param name="buf"></param>
		/// <param name="currentKeys"></param>
		/// <returns></returns>
		private static byte[] _encrypt(byte[] buf)
		{
			int size = buf.Length;
			char[] ek = UChar8.FromArray(_initKeys.encodeKey);
	
			buf[0] = (byte) (buf[0] ^ ek[0]); // 疑點
	
			for (int i = 1; i < size; i++)
			{
				buf[i] ^= (byte) (buf[i - 1] ^ ek[i & 7]);
			}
	
			buf[3] = (byte) (buf[3] ^ ek[2]);
			buf[2] = (byte) (buf[2] ^ buf[3] ^ ek[3]);
			buf[1] = (byte) (buf[1] ^ buf[2] ^ ek[4]);
			buf[0] = (byte) (buf[0] ^ buf[1] ^ ek[5]);
	
			return buf;
		}
	
		/// <summary>
		/// 解密 char[]
		/// </summary>
		///
		/// <param name="buf"></param>
		/// <param name="currentKeys"></param>
		/// <returns></returns>
		/// <exception cref="NoEncryptionKeysSelectedException"></exception>
		public static char[] Decrypt(char[] buf)
		{
			if (_initKeys == null)
			{
				throw new NoKeySelected();
			}
	
			_decrypt(buf);
	
			long mask = ULong32.FromArray(buf);
	
			_initKeys.decodeKey[0] ^= mask;
			_initKeys.decodeKey[1] = ULong32.Add(_initKeys.decodeKey[1], 0x287EFFC3L);
	
			return buf;
		}
	
		/// <summary>
		/// 解密處理
		/// </summary>
		///
		/// <param name="buf"></param>
		/// <param name="currentKeys"></param>
		/// <returns></returns>
		private static char[] _decrypt(char[] buf)
		{
			int size = buf.Length;
			char[] dk = UChar8.FromArray(_initKeys.decodeKey);
	
			char b3 = buf[3];
			buf[3] ^= dk[2];
	
			char b2 = buf[2];
			buf[2] ^= ((Char)(b3 ^ dk[3]));
	
			char b1 = buf[1];
			buf[1] ^= ((Char)(b2 ^ dk[4]));
	
			char k = (char) (buf[0] ^ b1 ^ dk[5]);
			buf[0] = (char) (k ^ dk[0]);
	
			for (int i = 1; i < size; i++)
			{
				char t = buf[i];
				buf[i] ^= ((Char)(dk[i & 7] ^ k));
				k = t;
			}
			
			return buf;
		}
	
		/// <summary>
		/// 解密
		/// </summary>
		///
		/// <param name="buf">byte[]</param>
		/// <param name="length"></param>
		/// <param name="currentKeys"></param>
		/// <returns></returns>
		/// <exception cref="NoEncryptionKeysSelectedException"></exception>
		public static byte[] Decrypt(byte[] buf, int length)
		{
			if (_initKeys == null)
			{
				throw new NoKeySelected();
			}
	
			_decrypt(buf, length);
	
			long mask = ULong32.FromArray(buf);
	
			_initKeys.decodeKey[0] ^= mask;
			_initKeys.decodeKey[1] = ULong32.Add(_initKeys.decodeKey[1], 0x287EFFC3L);
	
			return buf;
		}
	
		/// <summary>
		/// 解密處理
		/// </summary>
		///
		/// <param name="buf"></param>
		/// <param name="size"></param>
		/// <param name="currentKeys"></param>
		/// <returns></returns>
		private static byte[] _decrypt(byte[] buf, int size)
		{
			char[] dk = UChar8.FromArray(_initKeys.decodeKey);
	
			byte b3 = buf[3];
			buf[3] =  (byte) (buf[3] ^ dk[2]);
	
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

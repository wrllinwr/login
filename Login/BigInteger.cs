using System;

namespace SFL
{
	public class BigInteger
	{
		private const int _maxLength = 70;
		
		private uint[] _data = null;
		
		private int _dataLength;

		private static readonly int[] PRIMESBELOW2000 =
		{ 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
			101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
			211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
			307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
			401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
			503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
			601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
			701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
			809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
			907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
			1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
			1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
			1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
			1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
			1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
			1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
			1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
			1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
			1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
			1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999 };

		public BigInteger()
		{
			_data = new uint[_maxLength];
			_dataLength = 1;
		}

		public BigInteger(long value)
		{
			_data = new uint[_maxLength];
			long tempVal = value;

			_dataLength = 0;
			while (value != 0 && _dataLength < _maxLength)
			{
				_data[_dataLength] = (uint)(value & 0xFFFFFFFF);
				value >>= 32;
				_dataLength++;
			}

			if (tempVal > 0)
			{
				if (value != 0 || (_data[_maxLength-1] & 0x80000000) != 0)
				{
					throw(new ArithmeticException("Positive overflow in constructor."));
				}
			}
			else if (tempVal < 0)
			{
				if (value != -1 || (_data[_dataLength-1] & 0x80000000) == 0)
				{
					throw(new ArithmeticException("Negative underflow in constructor."));
				}
			}

			if (_dataLength == 0)
			{
				_dataLength = 1;
			}
		}

		public BigInteger(ulong value)
		{
			_data = new uint[_maxLength];
			_dataLength = 0;
			
			while (value != 0 && _dataLength < _maxLength)
			{
				_data[_dataLength] = (uint)(value & 0xFFFFFFFF);
				value >>= 32;
				_dataLength++;
			}

			if (value != 0 || (_data[_maxLength-1] & 0x80000000) != 0)
			{
				throw(new ArithmeticException("Positive overflow in constructor."));
			}

			if (_dataLength == 0)
			{
				_dataLength = 1;
			}
		}

		public BigInteger(BigInteger bi)
		{
			_data = new uint[_maxLength];
			_dataLength = bi._dataLength;
			
			for (int i = 0; i < _dataLength; i++)
			{
				_data[i] = bi._data[i];
			}
		}

		public BigInteger(string value, int radix)
		{
			BigInteger multiplier = new BigInteger(1);
			BigInteger result = new BigInteger();
			value = (value.ToUpper()).Trim();
			int limit = 0;
			
			if (value[0] == '-')
			{
				limit = 1;
			}
			
			for (int i = value.Length - 1; i >= limit ; i--)
			{
				int posVal = (int)value[i];
				
				if (posVal >= '0' && posVal <= '9')
				{
					posVal -= '0';
				}
				else if (posVal >= 'A' && posVal <= 'Z')
				{
					posVal = (posVal - 'A') + 10;
				}
				else
				{
					posVal = 9999999;
				}
				
				if (posVal >= radix)
				{
					throw(new ArithmeticException("Invalid string in constructor."));
				}
				else
				{
					if (value[0] == '-')
					{
						posVal = -posVal;
					}
					
					result = result + (multiplier * posVal);
					
					if ((i - 1) >= limit)
					{
						multiplier = multiplier * radix;
					}
				}
			}

			
			if (value[0] == '-')
			{
				if ((result._data[_maxLength-1] & 0x80000000) == 0)
				{
					throw(new ArithmeticException("Negative underflow in constructor."));
				}
				
			}
			else
			{
				if ((result._data[_maxLength-1] & 0x80000000) != 0)
				{
					throw(new ArithmeticException("Positive overflow in constructor."));
				}
				
			}

			_data = new uint[_maxLength];
			
			for (int i = 0; i < result._dataLength; i++)
			{
				_data[i] = result._data[i];
			}
			
			_dataLength = result._dataLength;
		}

		public BigInteger(byte[] inData)
		{
			_dataLength = inData.Length >> 2;
			int leftOver = inData.Length & 0x3;
			
			if (leftOver != 0)
			{
				_dataLength++;
			}
			
			if (_dataLength > _maxLength)
			{
				throw(new ArithmeticException("Byte overflow in constructor."));
			}
			
			_data = new uint[_maxLength];

			
			for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
			{
				_data[j] = (uint)((inData[i-3] << 24) + (inData[i-2] << 16) + (inData[i-1] <<  8) + inData[i]);
			}

			
			if (leftOver == 1)
			{
				_data[_dataLength-1] = (uint)inData[0];
			}
			else if (leftOver == 2)
			{
				_data[_dataLength-1] = (uint)((inData[0] << 8) + inData[1]);
			}
			else if (leftOver == 3)
			{
				_data[_dataLength-1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
			}
			
			while (_dataLength > 1 && _data[_dataLength-1] == 0)
			{
				_dataLength--;
			}
		}

		public BigInteger(byte[] inData, int inLen)
		{
			_dataLength = inLen >> 2;
			int leftOver = inLen & 0x3;
			
			if (leftOver != 0)
			{
				_dataLength++;
			}
			
			if (_dataLength > _maxLength || inLen > inData.Length)
			{
				throw(new ArithmeticException("Byte overflow in constructor."));
			}

			_data = new uint[_maxLength];

			
			for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
			{
				_data[j] = (uint)((inData[i-3] << 24) + (inData[i-2] << 16) + (inData[i-1] <<  8) + inData[i]);
			}
			
			if (leftOver == 1)
			{
				_data[_dataLength-1] = (uint)inData[0];
			}
			else if (leftOver == 2)
			{
				_data[_dataLength-1] = (uint)((inData[0] << 8) + inData[1]);
			}
			else if (leftOver == 3)
			{
				_data[_dataLength-1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
			}
			
			if (_dataLength == 0)
			{
				_dataLength = 1;
			}

			
			while (_dataLength > 1 && _data[_dataLength-1] == 0)
			{
				_dataLength--;
			}
		}

		public BigInteger(uint[] inData)
		{
			_dataLength = inData.Length;

			if (_dataLength > _maxLength)
			{
				throw(new ArithmeticException("Byte overflow in constructor."));
			}

			_data = new uint[_maxLength];

			for (int i = _dataLength - 1, j = 0; i >= 0; i--, j++)
			{
				_data[j] = inData[i];
			}

			while (_dataLength > 1 && _data[_dataLength-1] == 0)
			{
				_dataLength--;
			}
		}

		public static implicit operator BigInteger(long value)
		{
			return (new BigInteger(value));
		}

		public static implicit operator BigInteger(ulong value)
		{
			return (new BigInteger(value));
		}

		public static implicit operator BigInteger(int value)
		{
			return (new BigInteger((long)value));
		}

		public static implicit operator BigInteger(uint value)
		{
			return (new BigInteger((ulong)value));
		}

		public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();
			result._dataLength = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

			long carry = 0;
			for (int i = 0; i < result._dataLength; i++)
			{
				long sum = (long)bi1._data[i] + (long)bi2._data[i] + carry;
				carry  = sum >> 32;
				result._data[i] = (uint)(sum & 0xFFFFFFFF);
			}

			if (carry != 0 && result._dataLength < _maxLength)
			{
				result._data[result._dataLength] = (uint)(carry);
				result._dataLength++;
			}

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
			{
				result._dataLength--;
			}

			int lastPos = _maxLength - 1;
			if ((bi1._data[lastPos] & 0x80000000) == (bi2._data[lastPos] & 0x80000000) &&
			        (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException());
			}

			return result;
		}

		public static BigInteger operator ++(BigInteger bi1)
		{
			BigInteger result = new BigInteger(bi1);

			long val, carry = 1;
			int index = 0;

			while (carry != 0 && index < _maxLength)
			{
				val = (long)(result._data[index]);
				val++;

				result._data[index] = (uint)(val & 0xFFFFFFFF);
				carry = val >> 32;

				index++;
			}

			if (index > result._dataLength)
			{
				result._dataLength = index;
			}
			else
			{
				while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
				{
					result._dataLength--;
				}
			}

			int lastPos = _maxLength - 1;

			if ((bi1._data[lastPos] & 0x80000000) == 0 &&
			        (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException("Overflow in ++."));
			}
			
			return result;
		}

		public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			result._dataLength = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

			long carryIn = 0;
			for (int i = 0; i < result._dataLength; i++)
			{
				long diff;

				diff = (long)bi1._data[i] - (long)bi2._data[i] - carryIn;
				result._data[i] = (uint)(diff & 0xFFFFFFFF);

				if (diff < 0)
				{
					carryIn = 1;
				}
				else
				{
					carryIn = 0;
				}
			}

			if (carryIn != 0)
			{
				for (int i = result._dataLength; i < _maxLength; i++)
				{
					result._data[i] = 0xFFFFFFFF;
				}
				
				result._dataLength = _maxLength;
			}

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
			{
				result._dataLength--;
			}

			int lastPos = _maxLength - 1;
			if ((bi1._data[lastPos] & 0x80000000) != (bi2._data[lastPos] & 0x80000000) &&
			        (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException());
			}

			return result;
		}

		public static BigInteger operator --(BigInteger bi1)
		{
			BigInteger result = new BigInteger(bi1);

			long val;
			bool carryIn = true;
			int index = 0;

			while (carryIn && index < _maxLength)
			{
				val = (long)(result._data[index]);
				val--;

				result._data[index] = (uint)(val & 0xFFFFFFFF);

				if (val >= 0)
				{
					carryIn = false;
				}

				index++;
			}

			if (index > result._dataLength)
			{
				result._dataLength = index;
			}

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
			{
				result._dataLength--;
			}

			int lastPos = _maxLength - 1;

			if ((bi1._data[lastPos] & 0x80000000) != 0 &&
			        (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException("Underflow in --."));
			}

			return result;
		}

		public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
		{
			int lastPos = _maxLength-1;
			bool bi1Neg = false, bi2Neg = false;

			try
			{
				if ((bi1._data[lastPos] & 0x80000000) != 0)
				{
					bi1Neg = true; bi1 = -bi1;
				}
				if ((bi2._data[lastPos] & 0x80000000) != 0)
				{
					bi2Neg = true; bi2 = -bi2;
				}
			}
			catch(Exception) {}

			BigInteger result = new BigInteger();

			try
			{
				for (int i = 0; i < bi1._dataLength; i++)
				{
					if (bi1._data[i] == 0)
					{
						continue;
					}

					ulong mcarry = 0;
					for (int j = 0, k = i; j < bi2._dataLength; j++, k++)
					{
						// k = i + j
						ulong val = ((ulong)bi1._data[i] * (ulong)bi2._data[j]) + (ulong)result._data[k] + mcarry;

						result._data[k] = (uint)(val & 0xFFFFFFFF);
						mcarry = (val >> 32);
					}

					if (mcarry != 0)
					{
						result._data[i+bi2._dataLength] = (uint)mcarry;
					}
				}
			}
			catch(Exception)
			{
				throw(new ArithmeticException("Multiplication overflow."));
			}


			result._dataLength = bi1._dataLength + bi2._dataLength;
			if (result._dataLength > _maxLength)
			{
				result._dataLength = _maxLength;
			}

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
			{
				result._dataLength--;
			}

			if ((result._data[lastPos] & 0x80000000) != 0)
			{
				if (bi1Neg != bi2Neg && result._data[lastPos] == 0x80000000)
				{
					if (result._dataLength == 1)
					{
						return result;
					}
					else
					{
						bool isMaxNeg = true;
						for (int i = 0; i < result._dataLength - 1 && isMaxNeg; i++)
						{
							if (result._data[i] != 0)
							{
								isMaxNeg = false;
							}
						}

						if (isMaxNeg)
						{
							return result;
						}
					}
				}

				throw(new ArithmeticException("Multiplication overflow."));
			}

			if (bi1Neg != bi2Neg)
			{
				return -result;
			}

			return result;
		}

		public static BigInteger operator <<(BigInteger bi1, int shiftVal)
		{
			BigInteger result = new BigInteger(bi1);
			result._dataLength = shiftLeft(result._data, shiftVal);

			return result;
		}

		private static int shiftLeft(uint[] buffer, int shiftVal)
		{
			int shiftAmount = 32;
			int bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen-1] == 0)
			{
				bufLen--;
			}

			for (int count = shiftVal; count > 0;)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
				}

				ulong carry = 0;
				for (int i = 0; i < bufLen; i++)
				{
					ulong val = ((ulong)buffer[i]) << shiftAmount;
					val |= carry;

					buffer[i] = (uint)(val & 0xFFFFFFFF);
					carry = val >> 32;
				}

				if (carry != 0)
				{
					if (bufLen + 1 <= buffer.Length)
					{
						buffer[bufLen] = (uint)carry;
						bufLen++;
					}
				}
				
				count -= shiftAmount;
			}
			
			return bufLen;
		}

		public static BigInteger operator >>(BigInteger bi1, int shiftVal)
		{
			BigInteger result = new BigInteger(bi1);
			result._dataLength = shiftRight(result._data, shiftVal);

			if ((bi1._data[_maxLength-1] & 0x80000000) != 0)
			{
				for (int i = _maxLength - 1; i >= result._dataLength; i--)
				{
					result._data[i] = 0xFFFFFFFF;
				}

				uint mask = 0x80000000;
				for (int i = 0; i < 32; i++)
				{
					if ((result._data[result._dataLength-1] & mask) != 0)
					{
						break;
					}

					result._data[result._dataLength-1] |= mask;
					mask >>= 1;
				}
				
				result._dataLength = _maxLength;
			}

			return result;
		}


		private static int shiftRight(uint[] buffer, int shiftVal)
		{
			int shiftAmount = 32;
			int invShift = 0;
			int bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen-1] == 0)
			{
				bufLen--;
			}

			for (int count = shiftVal; count > 0;)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
					invShift = 32 - shiftAmount;
				}

				ulong carry = 0;
				for (int i = bufLen - 1; i >= 0; i--)
				{
					ulong val = ((ulong)buffer[i]) >> shiftAmount;
					val |= carry;

					carry = ((ulong)buffer[i]) << invShift;
					buffer[i] = (uint)(val);
				}

				count -= shiftAmount;
			}

			while (bufLen > 1 && buffer[bufLen-1] == 0)
			{
				bufLen--;
			}

			return bufLen;
		}

		public static BigInteger operator ~(BigInteger bi1)
		{
			BigInteger result = new BigInteger(bi1);

			for (int i = 0; i < _maxLength; i++)
			{
				result._data[i] = (uint)(~(bi1._data[i]));
			}

			result._dataLength = _maxLength;

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
			{
				result._dataLength--;
			}

			return result;
		}

		public static BigInteger operator -(BigInteger bi1)
		{
			if (bi1._dataLength == 1 && bi1._data[0] == 0)
			{
				return (new BigInteger());
			}

			BigInteger result = new BigInteger(bi1);

			for (int i = 0; i < _maxLength; i++)
			{
				result._data[i] = (uint)(~(bi1._data[i]));
			}
			
			long val, carry = 1;
			int index = 0;

			while (carry != 0 && index < _maxLength)
			{
				val = (long)(result._data[index]);
				val++;

				result._data[index] = (uint)(val & 0xFFFFFFFF);
				carry = val >> 32;

				index++;
			}

			if ((bi1._data[_maxLength-1] & 0x80000000) == (result._data[_maxLength-1] & 0x80000000))
			{
				throw (new ArithmeticException("Overflow in negation.\n"));
			}

			result._dataLength = _maxLength;

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
			{
				result._dataLength--;
			}
			
			return result;
		}

		public static bool operator ==(BigInteger bi1, BigInteger bi2)
		{
			return bi1.Equals(bi2);
		}


		public static bool operator !=(BigInteger bi1, BigInteger bi2)
		{
			return !(bi1.Equals(bi2));
		}

		public override bool Equals(object o)
		{
			BigInteger bi = (BigInteger)o;

			if (this._dataLength != bi._dataLength)
			{
				return false;
			}

			for (int i = 0; i < this._dataLength; i++)
			{
				if (this._data[i] != bi._data[i])
					return false;
			}
			
			return true;
		}


		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public static bool operator >(BigInteger bi1, BigInteger bi2)
		{
			int pos = _maxLength - 1;

			if ((bi1._data[pos] & 0x80000000) != 0 && (bi2._data[pos] & 0x80000000) == 0)
			{
				return false;
			}
			else if ((bi1._data[pos] & 0x80000000) == 0 && (bi2._data[pos] & 0x80000000) != 0)
			{
				return true;
			}

			int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
			for (pos = len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--);

			if (pos >= 0)
			{
				if (bi1._data[pos] > bi2._data[pos])
				{
					return true;
				}
				
				return false;
			}
			
			return false;
		}


		public static bool operator <(BigInteger bi1, BigInteger bi2)
		{
			int pos = _maxLength - 1;

			if ((bi1._data[pos] & 0x80000000) != 0 && (bi2._data[pos] & 0x80000000) == 0)
				return true;

			else if ((bi1._data[pos] & 0x80000000) == 0 && (bi2._data[pos] & 0x80000000) != 0)
				return false;

			int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
			for (pos = len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--);

			if (pos >= 0)
			{
				if (bi1._data[pos] < bi2._data[pos])
					return true;
				
				return false;
			}
			
			return false;
		}


		public static bool operator >=(BigInteger bi1, BigInteger bi2)
		{
			return (bi1 == bi2 || bi1 > bi2);
		}


		public static bool operator <=(BigInteger bi1, BigInteger bi2)
		{
			return (bi1 == bi2 || bi1 < bi2);
		}

		private static void multiByteDivide(
			BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] result = new uint[_maxLength];

			int remainderLen = bi1._dataLength + 1;
			uint[] remainder = new uint[remainderLen];

			uint mask = 0x80000000;
			uint val = bi2._data[bi2._dataLength - 1];
			int shift = 0, resultPos = 0;

			while (mask != 0 && (val & mask) == 0)
			{
				shift++;
				mask >>= 1;
			}

			for (int i = 0; i < bi1._dataLength; i++)
			{
				remainder[i] = bi1._data[i];
			}
			shiftLeft(remainder, shift);
			bi2 = bi2 << shift;

			int j = remainderLen - bi2._dataLength;
			int pos = remainderLen - 1;

			ulong firstDivisorByte = bi2._data[bi2._dataLength-1];
			ulong secondDivisorByte = bi2._data[bi2._dataLength-2];

			int divisorLen = bi2._dataLength + 1;
			uint[] dividendPart = new uint[divisorLen];

			while (j > 0)
			{
				ulong dividend = ((ulong)remainder[pos] << 32) + (ulong)remainder[pos-1];
				//Console.WriteLine("dividend = {0}", dividend);

				ulong q_hat = dividend / firstDivisorByte;
				ulong r_hat = dividend % firstDivisorByte;

				//Console.WriteLine("q_hat = {0:X}, r_hat = {1:X}", q_hat, r_hat);

				bool done = false;
				while (!done)
				{
					done = true;

					if (q_hat == 0x100000000 || (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos-2]))
					{
						q_hat--;
						r_hat += firstDivisorByte;

						if (r_hat < 0x100000000)
							done = false;
					}
				}

				for (int h = 0; h < divisorLen; h++)
				{
					dividendPart[h] = remainder[pos-h];
				}

				BigInteger kk = new BigInteger(dividendPart);
				BigInteger ss = bi2 * (long)q_hat;

				//Console.WriteLine("ss before = " + ss);
				while (ss > kk)
				{
					q_hat--;
					ss -= bi2;
					//Console.WriteLine(ss);
				}
				BigInteger yy = kk - ss;

				//Console.WriteLine("ss = " + ss);
				//Console.WriteLine("kk = " + kk);
				//Console.WriteLine("yy = " + yy);

				for (int h = 0; h < divisorLen; h++)
				{
					remainder[pos-h] = yy._data[bi2._dataLength-h];
				}

				/*
                        Console.WriteLine("dividend = ");
                        for (int q = remainderLen - 1; q >= 0; q--)
                                Console.Write("{0:x2}", remainder[q]);
                        Console.WriteLine("\n************ q_hat = {0:X}\n", q_hat);
				 */

				result[resultPos++] = (uint)q_hat;

				pos--;
				j--;
			}

			outQuotient._dataLength = resultPos;
			int y = 0;
			for (int x = outQuotient._dataLength - 1; x >= 0; x--, y++)
			{
				outQuotient._data[y] = result[x];
			}
			for (; y < _maxLength; y++)
			{
				outQuotient._data[y] = 0;
			}

			while (outQuotient._dataLength > 1 && outQuotient._data[outQuotient._dataLength-1] == 0)
			{
				outQuotient._dataLength--;
			}

			if (outQuotient._dataLength == 0)
			{
				outQuotient._dataLength = 1;
			}

			outRemainder._dataLength = shiftRight(remainder, shift);

			for (y = 0; y < outRemainder._dataLength; y++)
			{
				outRemainder._data[y] = remainder[y];
			}
			
			for (; y < _maxLength; y++)
			{
				outRemainder._data[y] = 0;
			}
		}

		private static void singleByteDivide(
			BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] result = new uint[_maxLength];
			int resultPos = 0;

			// copy dividend to reminder
			for (int i = 0; i < _maxLength; i++)
			{
				outRemainder._data[i] = bi1._data[i];
			}
			
			outRemainder._dataLength = bi1._dataLength;

			while (outRemainder._dataLength > 1 && outRemainder._data[outRemainder._dataLength-1] == 0)
			{
				outRemainder._dataLength--;
			}

			ulong divisor = (ulong)bi2._data[0];
			int pos = outRemainder._dataLength - 1;
			ulong dividend = (ulong)outRemainder._data[pos];

			//Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
			//Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);

			if (dividend >= divisor)
			{
				ulong quotient = dividend / divisor;
				result[resultPos++] = (uint)quotient;

				outRemainder._data[pos] = (uint)(dividend % divisor);
			}
			pos--;

			while (pos >= 0)
			{
				//Console.WriteLine(pos);

				dividend = ((ulong)outRemainder._data[pos+1] << 32) + (ulong)outRemainder._data[pos];
				ulong quotient = dividend / divisor;
				result[resultPos++] = (uint)quotient;

				outRemainder._data[pos+1] = 0;
				outRemainder._data[pos--] = (uint)(dividend % divisor);
				//Console.WriteLine(">>>> " + bi1);
			}

			outQuotient._dataLength = resultPos;
			int j = 0;
			for (int i = outQuotient._dataLength - 1; i >= 0; i--, j++)
			{
				outQuotient._data[j] = result[i];
			}
			for (; j < _maxLength; j++)
			{
				outQuotient._data[j] = 0;
			}

			while (outQuotient._dataLength > 1 && outQuotient._data[outQuotient._dataLength-1] == 0)
			{
				outQuotient._dataLength--;
			}

			if (outQuotient._dataLength == 0)
			{
				outQuotient._dataLength = 1;
			}

			while (outRemainder._dataLength > 1 && outRemainder._data[outRemainder._dataLength-1] == 0)
			{
				outRemainder._dataLength--;
			}
		}

		public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
		{
			BigInteger quotient = new BigInteger();
			BigInteger remainder = new BigInteger();

			int lastPos = _maxLength-1;
			bool divisorNeg = false, dividendNeg = false;

			if ((bi1._data[lastPos] & 0x80000000) != 0)     // bi1 negative
			{
				bi1 = -bi1;
				dividendNeg = true;
			}
			if ((bi2._data[lastPos] & 0x80000000) != 0)     // bi2 negative
			{
				bi2 = -bi2;
				divisorNeg = true;
			}

			if (bi1 < bi2)
			{
				return quotient;
			}
			else
			{
				if (bi2._dataLength == 1)
				{
					singleByteDivide(bi1, bi2, quotient, remainder);
				}
				else
				{
					multiByteDivide(bi1, bi2, quotient, remainder);
				}

				if (dividendNeg != divisorNeg)
				{
					return -quotient;
				}

				return quotient;
			}
		}

		public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
		{
			BigInteger quotient = new BigInteger();
			BigInteger remainder = new BigInteger(bi1);

			int lastPos = _maxLength-1;
			bool dividendNeg = false;

			if ((bi1._data[lastPos] & 0x80000000) != 0)     // bi1 negative
			{
				bi1 = -bi1;
				dividendNeg = true;
			}
			if ((bi2._data[lastPos] & 0x80000000) != 0)     // bi2 negative
			{
				bi2 = -bi2;
			}

			if (bi1 < bi2)
			{
				return remainder;
			}

			else
			{
				if (bi2._dataLength == 1)
					singleByteDivide(bi1, bi2, quotient, remainder);
				else
					multiByteDivide(bi1, bi2, quotient, remainder);

				if (dividendNeg)
					return -remainder;

				return remainder;
			}
		}

		public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

			for (int i = 0; i < len; i++)
			{
				uint sum = (uint)(bi1._data[i] & bi2._data[i]);
				result._data[i] = sum;
			}

			result._dataLength = _maxLength;

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
				result._dataLength--;

			return result;
		}

		public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

			for (int i = 0; i < len; i++)
			{
				uint sum = (uint)(bi1._data[i] | bi2._data[i]);
				result._data[i] = sum;
			}

			result._dataLength = _maxLength;

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
				result._dataLength--;

			return result;
		}

		public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

			for (int i = 0; i < len; i++)
			{
				uint sum = (uint)(bi1._data[i] ^ bi2._data[i]);
				result._data[i] = sum;
			}

			result._dataLength = _maxLength;

			while (result._dataLength > 1 && result._data[result._dataLength-1] == 0)
				result._dataLength--;

			return result;
		}

		public BigInteger max(BigInteger bi)
		{
			if (this > bi)
				return (new BigInteger(this));
			else
				return (new BigInteger(bi));
		}

		public BigInteger min(BigInteger bi)
		{
			if (this < bi)
				return (new BigInteger(this));
			else
				return (new BigInteger(bi));
		}


		//***********************************************************************
		// Returns the absolute value
		//***********************************************************************

		public BigInteger abs()
		{
			if ((this._data[_maxLength - 1] & 0x80000000) != 0)
				return (-this);
			else
				return (new BigInteger(this));
		}


		//***********************************************************************
		// Returns a string representing the BigInteger in base 10.
		//***********************************************************************

		public override string ToString()
		{
			return ToString(10);
		}


		//***********************************************************************
		// Returns a string representing the BigInteger in sign-and-magnitude
		// format in the specified radix.
		//
		// Example
		// -------
		// If the value of BigInteger is -255 in base 10, then
		// ToString(16) returns "-FF"
		//
		//***********************************************************************

		public string ToString(int radix)
		{
			if (radix < 2 || radix > 36)
				throw (new ArgumentException("Radix must be >= 2 and <= 36"));

			string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string result = "";

			BigInteger a = this;

			bool negative = false;
			if ((a._data[_maxLength-1] & 0x80000000) != 0)
			{
				negative = true;
				try
				{
					a = -a;
				}
				catch (Exception) {}
			}

			BigInteger quotient = new BigInteger();
			BigInteger remainder = new BigInteger();
			BigInteger biRadix = new BigInteger(radix);

			if (a._dataLength == 1 && a._data[0] == 0)
				result = "0";
			else
			{
				while (a._dataLength > 1 || (a._dataLength == 1 && a._data[0] != 0))
				{
					singleByteDivide(a, biRadix, quotient, remainder);

					if (remainder._data[0] < 10)
						result = remainder._data[0] + result;
					else
						result = charSet[(int)remainder._data[0] - 10] + result;

					a = quotient;
				}
				
				if (negative)
					result = "-" + result;
			}

			return result;
		}


		//***********************************************************************
		// Returns a hex string showing the contains of the BigInteger
		//
		// Examples
		// -------
		// 1) If the value of BigInteger is 255 in base 10, then
		//    ToHexString() returns "FF"
		//
		// 2) If the value of BigInteger is -255 in base 10, then
		//    ToHexString() returns ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
		//    which is the 2's complement representation of -255.
		//
		//***********************************************************************

		public string ToHexString()
		{
			string result = _data[_dataLength - 1].ToString("X");

			for (int i = _dataLength - 2; i >= 0; i--)
			{
				result += _data[i].ToString("X8");
			}

			return result;
		}



		//***********************************************************************
		// Modulo Exponentiation
		//***********************************************************************

		public BigInteger modPow(BigInteger exp, BigInteger n)
		{
			if ((exp._data[_maxLength-1] & 0x80000000) != 0)
				throw (new ArithmeticException("Positive exponents only."));

			BigInteger resultNum = 1;
			BigInteger tempNum;
			bool thisNegative = false;

			if ((this._data[_maxLength-1] & 0x80000000) != 0)   // negative this
			{
				tempNum = -this % n;
				thisNegative = true;
			}
			else
			{
				tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)
			}

			if ((n._data[_maxLength-1] & 0x80000000) != 0)   // negative n
				n = -n;

			// calculate constant = b^(2k) / m
			BigInteger constant = new BigInteger();

			int i = n._dataLength << 1;
			constant._data[i] = 0x00000001;
			constant._dataLength = i + 1;

			constant = constant / n;
			int totalBits = exp.bitCount();
			int count = 0;

			// perform squaring and multiply exponentiation
			for (int pos = 0; pos < exp._dataLength; pos++)
			{
				uint mask = 0x01;
				//Console.WriteLine("pos = " + pos);

				for (int index = 0; index < 32; index++)
				{
					if ((exp._data[pos] & mask) != 0)
						resultNum = BarrettReduction(resultNum * tempNum, n, constant);

					mask <<= 1;

					tempNum = BarrettReduction(tempNum * tempNum, n, constant);


					if (tempNum._dataLength == 1 && tempNum._data[0] == 1)
					{
						if (thisNegative && (exp._data[0] & 0x1) != 0)    //odd exp
							return -resultNum;
						
						return resultNum;
					}
					
					count++;
					if (count == totalBits)
						break;
				}
			}

			if (thisNegative && (exp._data[0] & 0x1) != 0)    //odd exp
				return -resultNum;

			return resultNum;
		}



		//***********************************************************************
		// Fast calculation of modular reduction using Barrett's reduction.
		// Requires x < b^(2k), where b is the base.  In this case, base is
		// 2^32 (uint).
		//
		// Reference [4]
		//***********************************************************************

		private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
		{
			int k = n._dataLength,
			kPlusOne = k+1,
			kMinusOne = k-1;

			BigInteger q1 = new BigInteger();

			// q1 = x / b^(k-1)
			for (int i = kMinusOne, j = 0; i < x._dataLength; i++, j++)
			{
				q1._data[j] = x._data[i];
			}
			q1._dataLength = x._dataLength - kMinusOne;
			if (q1._dataLength <= 0)
			{
				q1._dataLength = 1;
			}

			BigInteger q2 = q1 * constant;
			BigInteger q3 = new BigInteger();

			// q3 = q2 / b^(k+1)
			for (int i = kPlusOne, j = 0; i < q2._dataLength; i++, j++)
			{
				q3._data[j] = q2._data[i];
			}
			q3._dataLength = q2._dataLength - kPlusOne;
			if (q3._dataLength <= 0)
			{
				q3._dataLength = 1;
			}


			// r1 = x mod b^(k+1)
			// i.e. keep the lowest (k+1) words
			BigInteger r1 = new BigInteger();
			int lengthToCopy = (x._dataLength > kPlusOne) ? kPlusOne : x._dataLength;
			for (int i = 0; i < lengthToCopy; i++)
			{
				r1._data[i] = x._data[i];
			}
			r1._dataLength = lengthToCopy;


			// r2 = (q3 * n) mod b^(k+1)
			// partial multiplication of q3 and n

			BigInteger r2 = new BigInteger();
			for (int i = 0; i < q3._dataLength; i++)
			{
				if (q3._data[i] == 0)
				{
					continue;
				}

				ulong mcarry = 0;
				int t = i;
				for (int j = 0; j < n._dataLength && t < kPlusOne; j++, t++)
				{
					// t = i + j
					ulong val = ((ulong)q3._data[i] * (ulong)n._data[j]) + (ulong)r2._data[t] + mcarry;

					r2._data[t] = (uint)(val & 0xFFFFFFFF);
					mcarry = (val >> 32);
				}

				if (t < kPlusOne)
					r2._data[t] = (uint)mcarry;
			}
			r2._dataLength = kPlusOne;
			while (r2._dataLength > 1 && r2._data[r2._dataLength-1] == 0)
				r2._dataLength--;

			r1 -= r2;
			if ((r1._data[_maxLength-1] & 0x80000000) != 0)        // negative
			{
				BigInteger val = new BigInteger();
				val._data[kPlusOne] = 0x00000001;
				val._dataLength = kPlusOne + 1;
				r1 += val;
			}

			while (r1 >= n)
				r1 -= n;

			return r1;
		}


		//***********************************************************************
		// Returns gcd(this, bi)
		//***********************************************************************

		public BigInteger gcd(BigInteger bi)
		{
			BigInteger x;
			BigInteger y;

			if ((_data[_maxLength-1] & 0x80000000) != 0)     // negative
				x = -this;
			else
				x = this;

			if ((bi._data[_maxLength-1] & 0x80000000) != 0)     // negative
				y = -bi;
			else
				y = bi;

			BigInteger g = y;

			while (x._dataLength > 1 || (x._dataLength == 1 && x._data[0] != 0))
			{
				g = x;
				x = y % x;
				y = g;
			}

			return g;
		}


		//***********************************************************************
		// Populates "this" with the specified amount of random bits
		//***********************************************************************

		public void genRandomBits(int bits, Random rand)
		{
			int dwords = bits >> 5;
			int remBits = bits & 0x1F;

			if (remBits != 0)
				dwords++;

			if (dwords > _maxLength)
				throw (new ArithmeticException("Number of required bits > maxLength."));

			for (int i = 0; i < dwords; i++)
				_data[i] = (uint)(rand.NextDouble() * 0x100000000);

			for (int i = dwords; i < _maxLength; i++)
				_data[i] = 0;

			if (remBits != 0)
			{
				uint mask = (uint)(0x01 << (remBits-1));
				_data[dwords-1] |= mask;

				mask = (uint)(0xFFFFFFFF >> (32 - remBits));
				_data[dwords-1] &= mask;
			}
			else
			{
				_data[dwords-1] |= 0x80000000;
			}

			_dataLength = dwords;

			if (_dataLength == 0)
				_dataLength = 1;
		}


		//***********************************************************************
		// Returns the position of the most significant bit in the BigInteger.
		//
		// Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
		//      The result is 1, if the value of BigInteger is 0...0000 0001
		//      The result is 2, if the value of BigInteger is 0...0000 0010
		//      The result is 2, if the value of BigInteger is 0...0000 0011
		//
		//***********************************************************************

		public int bitCount()
		{
			while (_dataLength > 1 && _data[_dataLength-1] == 0)
				_dataLength--;

			uint value = _data[_dataLength - 1];
			uint mask = 0x80000000;
			int bits = 32;

			while (bits > 0 && (value & mask) == 0)
			{
				bits--;
				mask >>= 1;
			}
			bits += ((_dataLength - 1) << 5);

			return bits;
		}


		//***********************************************************************
		// Probabilistic prime test based on Fermat's little theorem
		//
		// for any a < p (p does not divide a) if
		//      a^(p-1) mod p != 1 then p is not prime.
		//
		// Otherwise, p is probably prime (pseudoprime to the chosen base).
		//
		// Returns
		// -------
		// True if "this" is a pseudoprime to randomly chosen
		// bases.  The number of chosen bases is given by the "confidence"
		// parameter.
		//
		// False if "this" is definitely NOT prime.
		//
		// Note - this method is fast but fails for Carmichael numbers except
		// when the randomly chosen base is a factor of the number.
		//
		//***********************************************************************

		public bool FermatLittleTest(int confidence)
		{
			BigInteger thisVal;
			if ((this._data[_maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if (thisVal._dataLength == 1)
			{
				// test small numbers
				if (thisVal._data[0] == 0 || thisVal._data[0] == 1)
					return false;
				else if (thisVal._data[0] == 2 || thisVal._data[0] == 3)
					return true;
			}

			if ((thisVal._data[0] & 0x1) == 0)     // even numbers
				return false;

			int bits = thisVal.bitCount();
			BigInteger a = new BigInteger();
			BigInteger p_sub1 = thisVal - (new BigInteger(1));
			Random rand = new Random();

			for (int round = 0; round < confidence; round++)
			{
				bool done = false;

				while (!done)		// generate a < n
				{
					int testBits = 0;

					// make sure "a" has at least 2 bits
					while (testBits < 2)
						testBits = (int)(rand.NextDouble() * bits);

					a.genRandomBits(testBits, rand);

					int byteLen = a._dataLength;

					// make sure "a" is not 0
					if (byteLen > 1 || (byteLen == 1 && a._data[0] != 1))
						done = true;
				}

				// check whether a factor exists (fix for version 1.03)
				BigInteger gcdTest = a.gcd(thisVal);
				if (gcdTest._dataLength == 1 && gcdTest._data[0] != 1)
					return false;

				// calculate a^(p-1) mod p
				BigInteger expResult = a.modPow(p_sub1, thisVal);

				int resultLen = expResult._dataLength;

				// is NOT prime is a^(p-1) mod p != 1

				if (resultLen > 1 || (resultLen == 1 && expResult._data[0] != 1))
				{
					//Console.WriteLine("a = " + a.ToString());
					return false;
				}
			}

			return true;
		}


		//***********************************************************************
		// Probabilistic prime test based on Rabin-Miller's
		//
		// for any p > 0 with p - 1 = 2^s * t
		//
		// p is probably prime (strong pseudoprime) if for any a < p,
		// 1) a^t mod p = 1 or
		// 2) a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
		//
		// Otherwise, p is composite.
		//
		// Returns
		// -------
		// True if "this" is a strong pseudoprime to randomly chosen
		// bases.  The number of chosen bases is given by the "confidence"
		// parameter.
		//
		// False if "this" is definitely NOT prime.
		//
		//***********************************************************************

		public bool RabinMillerTest(int confidence)
		{
			BigInteger thisVal;
			if ((this._data[_maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if (thisVal._dataLength == 1)
			{
				// test small numbers
				if (thisVal._data[0] == 0 || thisVal._data[0] == 1)
					return false;
				else if (thisVal._data[0] == 2 || thisVal._data[0] == 3)
					return true;
			}

			if ((thisVal._data[0] & 0x1) == 0)     // even numbers
				return false;


			// calculate values of s and t
			BigInteger p_sub1 = thisVal - (new BigInteger(1));
			int s = 0;

			for (int index = 0; index < p_sub1._dataLength; index++)
			{
				uint mask = 0x01;

				for (int i = 0; i < 32; i++)
				{
					if ((p_sub1._data[index] & mask) != 0)
					{
						index = p_sub1._dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = p_sub1 >> s;

			int bits = thisVal.bitCount();
			BigInteger a = new BigInteger();
			Random rand = new Random();

			for (int round = 0; round < confidence; round++)
			{
				bool done = false;

				while (!done)		// generate a < n
				{
					int testBits = 0;

					// make sure "a" has at least 2 bits
					while (testBits < 2)
						testBits = (int)(rand.NextDouble() * bits);

					a.genRandomBits(testBits, rand);

					int byteLen = a._dataLength;

					// make sure "a" is not 0
					if (byteLen > 1 || (byteLen == 1 && a._data[0] != 1))
						done = true;
				}

				// check whether a factor exists (fix for version 1.03)
				BigInteger gcdTest = a.gcd(thisVal);
				if (gcdTest._dataLength == 1 && gcdTest._data[0] != 1)
					return false;

				BigInteger b = a.modPow(t, thisVal);

				/*
                        Console.WriteLine("a = " + a.ToString(10));
                        Console.WriteLine("b = " + b.ToString(10));
                        Console.WriteLine("t = " + t.ToString(10));
                        Console.WriteLine("s = " + s);
				 */

				bool result = false;

				if (b._dataLength == 1 && b._data[0] == 1)         // a^t mod p = 1
					result = true;

				for (int j = 0; result == false && j < s; j++)
				{
					if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
					{
						result = true;
						break;
					}

					b = (b * b) % thisVal;
				}

				if (result == false)
					return false;
			}
			return true;
		}


		//***********************************************************************
		// Probabilistic prime test based on Solovay-Strassen (Euler Criterion)
		//
		// p is probably prime if for any a < p (a is not multiple of p),
		// a^((p-1)/2) mod p = J(a, p)
		//
		// where J is the Jacobi symbol.
		//
		// Otherwise, p is composite.
		//
		// Returns
		// -------
		// True if "this" is a Euler pseudoprime to randomly chosen
		// bases.  The number of chosen bases is given by the "confidence"
		// parameter.
		//
		// False if "this" is definitely NOT prime.
		//
		//***********************************************************************

		public bool SolovayStrassenTest(int confidence)
		{
			BigInteger thisVal;
			if ((this._data[_maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if (thisVal._dataLength == 1)
			{
				// test small numbers
				if (thisVal._data[0] == 0 || thisVal._data[0] == 1)
					return false;
				else if (thisVal._data[0] == 2 || thisVal._data[0] == 3)
					return true;
			}

			if ((thisVal._data[0] & 0x1) == 0)     // even numbers
				return false;


			int bits = thisVal.bitCount();
			BigInteger a = new BigInteger();
			BigInteger p_sub1 = thisVal - 1;
			BigInteger p_sub1_shift = p_sub1 >> 1;

			Random rand = new Random();

			for (int round = 0; round < confidence; round++)
			{
				bool done = false;

				while (!done)		// generate a < n
				{
					int testBits = 0;

					// make sure "a" has at least 2 bits
					while (testBits < 2)
						testBits = (int)(rand.NextDouble() * bits);

					a.genRandomBits(testBits, rand);

					int byteLen = a._dataLength;

					// make sure "a" is not 0
					if (byteLen > 1 || (byteLen == 1 && a._data[0] != 1))
						done = true;
				}

				// check whether a factor exists (fix for version 1.03)
				BigInteger gcdTest = a.gcd(thisVal);
				if (gcdTest._dataLength == 1 && gcdTest._data[0] != 1)
					return false;

				// calculate a^((p-1)/2) mod p

				BigInteger expResult = a.modPow(p_sub1_shift, thisVal);
				if (expResult == p_sub1)
					expResult = -1;

				// calculate Jacobi symbol
				BigInteger jacob = Jacobi(a, thisVal);

				//Console.WriteLine("a = " + a.ToString(10) + " b = " + thisVal.ToString(10));
				//Console.WriteLine("expResult = " + expResult.ToString(10) + " Jacob = " + jacob.ToString(10));

				// if they are different then it is not prime
				if (expResult != jacob)
					return false;
			}

			return true;
		}


		//***********************************************************************
		// Implementation of the Lucas Strong Pseudo Prime test.
		//
		// Let n be an odd number with gcd(n,D) = 1, and n - J(D, n) = 2^s * d
		// with d odd and s >= 0.
		//
		// If Ud mod n = 0 or V2^r*d mod n = 0 for some 0 <= r < s, then n
		// is a strong Lucas pseudoprime with parameters (P, Q).  We select
		// P and Q based on Selfridge.
		//
		// Returns True if number is a strong Lucus pseudo prime.
		// Otherwise, returns False indicating that number is composite.
		//***********************************************************************

		public bool LucasStrongTest()
		{
			BigInteger thisVal;
			if ((this._data[_maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if (thisVal._dataLength == 1)
			{
				// test small numbers
				if (thisVal._data[0] == 0 || thisVal._data[0] == 1)
					return false;
				else if (thisVal._data[0] == 2 || thisVal._data[0] == 3)
					return true;
			}

			if ((thisVal._data[0] & 0x1) == 0)     // even numbers
				return false;

			return LucasStrongTestHelper(thisVal);
		}


		private bool LucasStrongTestHelper(BigInteger thisVal)
		{
			// Do the test (selects D based on Selfridge)
			// Let D be the first element of the sequence
			// 5, -7, 9, -11, 13, ... for which J(D,n) = -1
			// Let P = 1, Q = (1-D) / 4

			long D = 5, sign = -1, dCount = 0;
			bool done = false;

			while (!done)
			{
				int Jresult = BigInteger.Jacobi(D, thisVal);

				if (Jresult == -1)
					done = true;    // J(D, this) = 1
				else
				{
					if (Jresult == 0 && Math.Abs(D) < thisVal)       // divisor found
						return false;

					if (dCount == 20)
					{
						// check for square
						BigInteger root = thisVal.sqrt();
						if (root * root == thisVal)
							return false;
					}

					//Console.WriteLine(D);
					D = (Math.Abs(D) + 2) * sign;
					sign = -sign;
				}
				dCount++;
			}

			long Q = (1 - D) >> 2;

			/*
                Console.WriteLine("D = " + D);
                Console.WriteLine("Q = " + Q);
                Console.WriteLine("(n,D) = " + thisVal.gcd(D));
                Console.WriteLine("(n,Q) = " + thisVal.gcd(Q));
                Console.WriteLine("J(D|n) = " + BigInteger.Jacobi(D, thisVal));
			 */

			BigInteger p_add1 = thisVal + 1;
			int s = 0;

			for (int index = 0; index < p_add1._dataLength; index++)
			{
				uint mask = 0x01;

				for (int i = 0; i < 32; i++)
				{
					if ((p_add1._data[index] & mask) != 0)
					{
						index = p_add1._dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = p_add1 >> s;

			// calculate constant = b^(2k) / m
			// for Barrett Reduction
			BigInteger constant = new BigInteger();

			int nLen = thisVal._dataLength << 1;
			constant._data[nLen] = 0x00000001;
			constant._dataLength = nLen + 1;

			constant = constant / thisVal;

			BigInteger[] lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
			bool isPrime = false;

			if ((lucas[0]._dataLength == 1 && lucas[0]._data[0] == 0) ||
			   (lucas[1]._dataLength == 1 && lucas[1]._data[0] == 0))
			{
				// u(t) = 0 or V(t) = 0
				isPrime = true;
			}

			for (int i = 1; i < s; i++)
			{
				if (!isPrime)
				{
					// doubling of index
					lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
					lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

					//lucas[1] = ((lucas[1] * lucas[1]) - (lucas[2] << 1)) % thisVal;

					if ((lucas[1]._dataLength == 1 && lucas[1]._data[0] == 0))
						isPrime = true;
				}

				lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
			}


			if (isPrime)     // additional checks for composite numbers
			{
				// If n is prime and gcd(n, Q) == 1, then
				// Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

				BigInteger g = thisVal.gcd(Q);
				if (g._dataLength == 1 && g._data[0] == 1)         // gcd(this, Q) == 1
				{
					if ((lucas[2]._data[_maxLength-1] & 0x80000000) != 0)
						lucas[2] += thisVal;

					BigInteger temp = (Q * BigInteger.Jacobi(Q, thisVal)) % thisVal;
					if ((temp._data[_maxLength-1] & 0x80000000) != 0)
						temp += thisVal;

					if (lucas[2] != temp)
						isPrime = false;
				}
			}

			return isPrime;
		}


		//***********************************************************************
		// Determines whether a number is probably prime, using the Rabin-Miller's
		// test.  Before applying the test, the number is tested for divisibility
		// by primes < 2000
		//
		// Returns true if number is probably prime.
		//***********************************************************************

		public bool isProbablePrime(int confidence)
		{
			BigInteger thisVal;
			if ((this._data[_maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;


			// test for divisibility by primes < 2000
			for (int p = 0; p < PRIMESBELOW2000.Length; p++)
			{
				BigInteger divisor = PRIMESBELOW2000[p];

				if (divisor >= thisVal)
					break;

				BigInteger resultNum = thisVal % divisor;
				if (resultNum.IntValue() == 0)
				{
					/*
				Console.WriteLine("Not prime!  Divisible by {0}\n",
                                                  primesBelow2000[p]);
					 */
					return false;
				}
			}

			if (thisVal.RabinMillerTest(confidence))
				return true;
			else
			{
				//Console.WriteLine("Not prime!  Failed primality test\n");
				return false;
			}
		}


		//***********************************************************************
		// Determines whether this BigInteger is probably prime using a
		// combination of base 2 strong pseudoprime test and Lucas strong
		// pseudoprime test.
		//
		// The sequence of the primality test is as follows,
		//
		// 1) Trial divisions are carried out using prime numbers below 2000.
		//    if any of the primes divides this BigInteger, then it is not prime.
		//
		// 2) Perform base 2 strong pseudoprime test.  If this BigInteger is a
		//    base 2 strong pseudoprime, proceed on to the next step.
		//
		// 3) Perform strong Lucas pseudoprime test.
		//
		// Returns True if this BigInteger is both a base 2 strong pseudoprime
		// and a strong Lucas pseudoprime.
		//
		// For a detailed discussion of this primality test, see [6].
		//
		//***********************************************************************

		public bool isProbablePrime()
		{
			BigInteger thisVal;
			if ((this._data[_maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if (thisVal._dataLength == 1)
			{
				// test small numbers
				if (thisVal._data[0] == 0 || thisVal._data[0] == 1)
					return false;
				else if (thisVal._data[0] == 2 || thisVal._data[0] == 3)
					return true;
			}

			if ((thisVal._data[0] & 0x1) == 0)     // even numbers
				return false;


			// test for divisibility by primes < 2000
			for (int p = 0; p < PRIMESBELOW2000.Length; p++)
			{
				BigInteger divisor = PRIMESBELOW2000[p];

				if (divisor >= thisVal)
					break;

				BigInteger resultNum = thisVal % divisor;
				if (resultNum.IntValue() == 0)
				{
					//Console.WriteLine("Not prime!  Divisible by {0}\n",
					//                  primesBelow2000[p]);

					return false;
				}
			}

			// Perform BASE 2 Rabin-Miller Test

			// calculate values of s and t
			BigInteger p_sub1 = thisVal - (new BigInteger(1));
			int s = 0;

			for (int index = 0; index < p_sub1._dataLength; index++)
			{
				uint mask = 0x01;

				for (int i = 0; i < 32; i++)
				{
					if ((p_sub1._data[index] & mask) != 0)
					{
						index = p_sub1._dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = p_sub1 >> s;

			int bits = thisVal.bitCount();
			BigInteger a = 2;

			// b = a^t mod p
			BigInteger b = a.modPow(t, thisVal);
			bool result = false;

			if (b._dataLength == 1 && b._data[0] == 1)         // a^t mod p = 1
				result = true;

			for (int j = 0; result == false && j < s; j++)
			{
				if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
				{
					result = true;
					break;
				}

				b = (b * b) % thisVal;
			}

			// if number is strong pseudoprime to base 2, then do a strong lucas test
			if (result)
				result = LucasStrongTestHelper(thisVal);

			return result;
		}



		//***********************************************************************
		// Returns the lowest 4 bytes of the BigInteger as an int.
		//***********************************************************************

		public int IntValue()
		{
			return (int)_data[0];
		}


		//***********************************************************************
		// Returns the lowest 8 bytes of the BigInteger as a long.
		//***********************************************************************

		public long LongValue()
		{
			long val = 0;

			val = (long)_data[0];
			try
			{       // exception if maxLength = 1
				val |= (long)_data[1] << 32;
			}
			catch(Exception)
			{
				if ((_data[0] & 0x80000000) != 0) // negative
					val = (int)_data[0];
			}

			return val;
		}


		//***********************************************************************
		// Computes the Jacobi Symbol for a and b.
		// Algorithm adapted from [3] and [4] with some optimizations
		//***********************************************************************

		public static int Jacobi(BigInteger a, BigInteger b)
		{
			// Jacobi defined only for odd integers
			if ((b._data[0] & 0x1) == 0)
				throw (new ArgumentException("Jacobi defined only for odd integers."));

			if (a >= b)      a %= b;
			if (a._dataLength == 1 && a._data[0] == 0)      return 0;  // a == 0
			if (a._dataLength == 1 && a._data[0] == 1)      return 1;  // a == 1

			if (a < 0)
			{
				if ((((b-1)._data[0]) & 0x2) == 0)       //if ( (((b-1) >> 1).data[0] & 0x1) == 0)
					return Jacobi(-a, b);
				else
					return -Jacobi(-a, b);
			}

			int e = 0;
			for (int index = 0; index < a._dataLength; index++)
			{
				uint mask = 0x01;

				for (int i = 0; i < 32; i++)
				{
					if ((a._data[index] & mask) != 0)
					{
						index = a._dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					e++;
				}
			}

			BigInteger a1 = a >> e;

			int s = 1;
			if ((e & 0x1) != 0 && ((b._data[0] & 0x7) == 3 || (b._data[0] & 0x7) == 5))
				s = -1;

			if ((b._data[0] & 0x3) == 3 && (a1._data[0] & 0x3) == 3)
				s = -s;

			if (a1._dataLength == 1 && a1._data[0] == 1)
				return s;
			else
				return (s * Jacobi(b % a1, a1));
		}



		//***********************************************************************
		// Generates a positive BigInteger that is probably prime.
		//***********************************************************************

		public static BigInteger genPseudoPrime(int bits, int confidence, Random rand)
		{
			BigInteger result = new BigInteger();
			bool done = false;

			while (!done)
			{
				result.genRandomBits(bits, rand);
				result._data[0] |= 0x01;		// make it odd

				// prime test
				done = result.isProbablePrime(confidence);
			}
			return result;
		}


		//***********************************************************************
		// Generates a random number with the specified number of bits such
		// that gcd(number, this) = 1
		//***********************************************************************

		public BigInteger genCoPrime(int bits, Random rand)
		{
			bool done = false;
			BigInteger result = new BigInteger();

			while (!done)
			{
				result.genRandomBits(bits, rand);
				//Console.WriteLine(result.ToString(16));

				// gcd test
				BigInteger g = result.gcd(this);
				if (g._dataLength == 1 && g._data[0] == 1)
					done = true;
			}

			return result;
		}


		//***********************************************************************
		// Returns the modulo inverse of this.  Throws ArithmeticException if
		// the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
		//***********************************************************************

		public BigInteger modInverse(BigInteger modulus)
		{
			BigInteger[] p = { 0, 1 };
			BigInteger[] q = new BigInteger[2];    // quotients
			BigInteger[] r = { 0, 0 };             // remainders

			int step = 0;

			BigInteger a = modulus;
			BigInteger b = this;

			while (b._dataLength > 1 || (b._dataLength == 1 && b._data[0] != 0))
			{
				BigInteger quotient = new BigInteger();
				BigInteger remainder = new BigInteger();

				if (step > 1)
				{
					BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
					p[0] = p[1];
					p[1] = pval;
				}

				if (b._dataLength == 1)
					singleByteDivide(a, b, quotient, remainder);
				else
					multiByteDivide(a, b, quotient, remainder);

				/*
                        Console.WriteLine(quotient.dataLength);
                        Console.WriteLine("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
                                          b.ToString(10), quotient.ToString(10), remainder.ToString(10),
                                          p[1].ToString(10));
				 */

				q[0] = q[1];
				r[0] = r[1];
				q[1] = quotient; r[1] = remainder;

				a = b;
				b = remainder;

				step++;
			}

			if (r[0]._dataLength > 1 || (r[0]._dataLength == 1 && r[0]._data[0] != 1))
				throw (new ArithmeticException("No inverse!"));

			BigInteger result = ((p[0] - (p[1] * q[0])) % modulus);

			if ((result._data[_maxLength - 1] & 0x80000000) != 0)
				result += modulus;  // get the least positive modulus

			return result;
		}


		//***********************************************************************
		// Returns the value of the BigInteger as a byte array.  The lowest
		// index contains the MSB.
		//***********************************************************************

		public byte[] getBytes()
		{
			int numBits = bitCount();

			int numBytes = numBits >> 3;
			if ((numBits & 0x7) != 0)
				numBytes++;

			byte[] result = new byte[numBytes];

			//Console.WriteLine(result.Length);

			int pos = 0;
			uint tempVal, val = _data[_dataLength - 1];

			if ((tempVal = (val >> 24 & 0xFF)) != 0)
				result[pos++] = (byte)tempVal;
			if ((tempVal = (val >> 16 & 0xFF)) != 0)
				result[pos++] = (byte)tempVal;
			if ((tempVal = (val >> 8 & 0xFF)) != 0)
				result[pos++] = (byte)tempVal;
			if ((tempVal = (val & 0xFF)) != 0)
				result[pos++] = (byte)tempVal;

			for (int i = _dataLength - 2; i >= 0; i--, pos += 4)
			{
				val = _data[i];
				result[pos+3] = (byte)(val & 0xFF);
				val >>= 8;
				result[pos+2] = (byte)(val & 0xFF);
				val >>= 8;
				result[pos+1] = (byte)(val & 0xFF);
				val >>= 8;
				result[pos] = (byte)(val & 0xFF);
			}

			return result;
		}


		//***********************************************************************
		// Sets the value of the specified bit to 1
		// The Least Significant Bit position is 0.
		//***********************************************************************

		public void setBit(uint bitNum)
		{
			uint bytePos = bitNum >> 5;             // divide by 32
			byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

			uint mask = (uint)1 << bitPos;
			this._data[bytePos] |= mask;

			if (bytePos >= this._dataLength)
				this._dataLength = (int)bytePos + 1;
		}


		//***********************************************************************
		// Sets the value of the specified bit to 0
		// The Least Significant Bit position is 0.
		//***********************************************************************

		public void unsetBit(uint bitNum)
		{
			uint bytePos = bitNum >> 5;

			if (bytePos < this._dataLength)
			{
				byte bitPos = (byte)(bitNum & 0x1F);

				uint mask = (uint)1 << bitPos;
				uint mask2 = 0xFFFFFFFF ^ mask;

				this._data[bytePos] &= mask2;

				if (this._dataLength > 1 && this._data[this._dataLength - 1] == 0)
					this._dataLength--;
			}
		}


		//***********************************************************************
		// Returns a value that is equivalent to the integer square root
		// of the BigInteger.
		//
		// The integer square root of "this" is defined as the largest integer n
		// such that (n * n) <= this
		//
		//***********************************************************************

		public BigInteger sqrt()
		{
			uint numBits = (uint)this.bitCount();

			if ((numBits & 0x1) != 0)        // odd number of bits
				numBits = (numBits >> 1) + 1;
			else
				numBits = (numBits >> 1);

			uint bytePos = numBits >> 5;
			byte bitPos = (byte)(numBits & 0x1F);

			uint mask;

			BigInteger result = new BigInteger();
			if (bitPos == 0)
				mask = 0x80000000;
			else
			{
				mask = (uint)1 << bitPos;
				bytePos++;
			}
			result._dataLength = (int)bytePos;

			for (int i = (int)bytePos - 1; i >= 0; i--)
			{
				while (mask != 0)
				{
					// guess
					result._data[i] ^= mask;

					// undo the guess if its square is larger than this
					if ((result * result) > this)
						result._data[i] ^= mask;

					mask >>= 1;
				}
				mask = 0x80000000;
			}
			return result;
		}


		//***********************************************************************
		// Returns the k_th number in the Lucas Sequence reduced modulo n.
		//
		// Uses index doubling to speed up the process.  For example, to calculate V(k),
		// we maintain two numbers in the sequence V(n) and V(n+1).
		//
		// To obtain V(2n), we use the identity
		//      V(2n) = (V(n) * V(n)) - (2 * Q^n)
		// To obtain V(2n+1), we first write it as
		//      V(2n+1) = V((n+1) + n)
		// and use the identity
		//      V(m+n) = V(m) * V(n) - Q * V(m-n)
		// Hence,
		//      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
		//                   = V(n+1) * V(n) - Q^n * V(1)
		//                   = V(n+1) * V(n) - Q^n * P
		//
		// We use k in its binary expansion and perform index doubling for each
		// bit position.  For each bit position that is set, we perform an
		// index doubling followed by an index addition.  This means that for V(n),
		// we need to update it to V(2n+1).  For V(n+1), we need to update it to
		// V((2n+1)+1) = V(2*(n+1))
		//
		// This function returns
		// [0] = U(k)
		// [1] = V(k)
		// [2] = Q^n
		//
		// Where U(0) = 0 % n, U(1) = 1 % n
		//       V(0) = 2 % n, V(1) = P % n
		//***********************************************************************

		public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q,
		                                         BigInteger k, BigInteger n)
		{
			if (k._dataLength == 1 && k._data[0] == 0)
			{
				BigInteger[] result = new BigInteger[3];

				result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
				return result;
			}

			// calculate constant = b^(2k) / m
			// for Barrett Reduction
			BigInteger constant = new BigInteger();

			int nLen = n._dataLength << 1;
			constant._data[nLen] = 0x00000001;
			constant._dataLength = nLen + 1;

			constant = constant / n;

			// calculate values of s and t
			int s = 0;

			for (int index = 0; index < k._dataLength; index++)
			{
				uint mask = 0x01;

				for (int i = 0; i < 32; i++)
				{
					if ((k._data[index] & mask) != 0)
					{
						index = k._dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = k >> s;

			//Console.WriteLine("s = " + s + " t = " + t);
			return LucasSequenceHelper(P, Q, t, n, constant, s);
		}


		//***********************************************************************
		// Performs the calculation of the kth term in the Lucas Sequence.
		// For details of the algorithm, see reference [9].
		//
		// k must be odd.  i.e LSB == 1
		//***********************************************************************

		private static BigInteger[] LucasSequenceHelper(BigInteger P, BigInteger Q,
		                                                BigInteger k, BigInteger n,
		                                                BigInteger constant, int s)
		{
			BigInteger[] result = new BigInteger[3];

			if ((k._data[0] & 0x00000001) == 0)
				throw (new ArgumentException("Argument k must be odd."));

			int numbits = k.bitCount();
			uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);

			// v = v0, v1 = v1, u1 = u1, Q_k = Q^0

			BigInteger v = 2 % n, Q_k = 1 % n,
			v1 = P % n, u1 = Q_k;
			bool flag = true;

			for (int i = k._dataLength - 1; i >= 0 ; i--)     // iterate on the binary expansion of k
			{
				//Console.WriteLine("round");
				while (mask != 0)
				{
					if (i == 0 && mask == 0x00000001)        // last bit
						break;

					if ((k._data[i] & mask) != 0)             // bit is set
					{
						// index doubling with addition

						u1 = (u1 * v1) % n;

						v = ((v * v1) - (P * Q_k)) % n;
						v1 = n.BarrettReduction(v1 * v1, n, constant);
						v1 = (v1 - ((Q_k * Q) << 1)) % n;

						if (flag)
							flag = false;
						else
							Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

						Q_k = (Q_k * Q) % n;
					}
					else
					{
						// index doubling
						u1 = ((u1 * v) - Q_k) % n;

						v1 = ((v * v1) - (P * Q_k)) % n;
						v = n.BarrettReduction(v * v, n, constant);
						v = (v - (Q_k << 1)) % n;

						if (flag)
						{
							Q_k = Q % n;
							flag = false;
						}
						else
							Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
					}

					mask >>= 1;
				}
				mask = 0x80000000;
			}

			// at this point u1 = u(n+1) and v = v(n)
			// since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

			u1 = ((u1 * v) - Q_k) % n;
			v = ((v * v1) - (P * Q_k)) % n;
			if (flag)
				flag = false;
			else
				Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

			Q_k = (Q_k * Q) % n;


			for (int i = 0; i < s; i++)
			{
				// index doubling
				u1 = (u1 * v) % n;
				v = ((v * v) - (Q_k << 1)) % n;

				if (flag)
				{
					Q_k = Q % n;
					flag = false;
				}
				else
					Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
			}

			result[0] = u1;
			result[1] = v;
			result[2] = Q_k;

			return result;
		}


		//***********************************************************************
		// Tests the correct implementation of the /, %, * and + operators
		//***********************************************************************

		public static void MulDivTest(int rounds)
		{
			Random rand = new Random();
			byte[] val = new byte[64];
			byte[] val2 = new byte[64];

			for (int count = 0; count < rounds; count++)
			{
				// generate 2 numbers of random length
				int t1 = 0;
				while (t1 == 0)
					t1 = (int)(rand.NextDouble() * 65);

				int t2 = 0;
				while (t2 == 0)
					t2 = (int)(rand.NextDouble() * 65);

				bool done = false;
				while (!done)
				{
					for (int i = 0; i < 64; i++)
					{
						if (i < t1)
							val[i] = (byte)(rand.NextDouble() * 256);
						else
							val[i] = 0;

						if (val[i] != 0)
							done = true;
					}
				}

				done = false;
				while (!done)
				{
					for (int i = 0; i < 64; i++)
					{
						if (i < t2)
							val2[i] = (byte)(rand.NextDouble() * 256);
						else
							val2[i] = 0;

						if (val2[i] != 0)
							done = true;
					}
				}

				while (val[0] == 0)
					val[0] = (byte)(rand.NextDouble() * 256);
				while (val2[0] == 0)
					val2[0] = (byte)(rand.NextDouble() * 256);

				Console.WriteLine(count);
				BigInteger bn1 = new BigInteger(val, t1);
				BigInteger bn2 = new BigInteger(val2, t2);


				// Determine the quotient and remainder by dividing
				// the first number by the second.

				BigInteger bn3 = bn1 / bn2;
				BigInteger bn4 = bn1 % bn2;

				// Recalculate the number
				BigInteger bn5 = (bn3 * bn2) + bn4;

				// Make sure they're the same
				if (bn5 != bn1)
				{
					Console.WriteLine("Error at " + count);
					Console.WriteLine(bn1 + "\n");
					Console.WriteLine(bn2 + "\n");
					Console.WriteLine(bn3 + "\n");
					Console.WriteLine(bn4 + "\n");
					Console.WriteLine(bn5 + "\n");
					return;
				}
			}
		}


		//***********************************************************************
		// Tests the correct implementation of the modulo exponential function
		// using RSA encryption and decryption (using pre-computed encryption and
		// decryption keys).
		//***********************************************************************

		public static void RSATest(int rounds)
		{
			Random rand = new Random(1);
			byte[] val = new byte[64];

			// private and public key
			BigInteger bi_e = new BigInteger("a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7", 16);
			BigInteger bi_d = new BigInteger("4adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7", 16);
			BigInteger bi_n = new BigInteger("e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f", 16);

			Console.WriteLine("e =\n" + bi_e.ToString(10));
			Console.WriteLine("\nd =\n" + bi_d.ToString(10));
			Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

			for (int count = 0; count < rounds; count++)
			{
				// generate data of random length
				int t1 = 0;
				while (t1 == 0)
					t1 = (int)(rand.NextDouble() * 65);

				bool done = false;
				while (!done)
				{
					for (int i = 0; i < 64; i++)
					{
						if (i < t1)
							val[i] = (byte)(rand.NextDouble() * 256);
						else
							val[i] = 0;

						if (val[i] != 0)
							done = true;
					}
				}

				while (val[0] == 0)
					val[0] = (byte)(rand.NextDouble() * 256);

				Console.Write("Round = " + count);

				// encrypt and decrypt data
				BigInteger bi_data = new BigInteger(val, t1);
				BigInteger bi_encrypted = bi_data.modPow(bi_e, bi_n);
				BigInteger bi_decrypted = bi_encrypted.modPow(bi_d, bi_n);

				// compare
				if (bi_decrypted != bi_data)
				{
					Console.WriteLine("\nError at round " + count);
					Console.WriteLine(bi_data + "\n");
					return;
				}
				Console.WriteLine(" <PASSED>.");
			}

		}


		//***********************************************************************
		// Tests the correct implementation of the modulo exponential and
		// inverse modulo functions using RSA encryption and decryption.  The two
		// pseudoprimes p and q are fixed, but the two RSA keys are generated
		// for each round of testing.
		//***********************************************************************

		public static void RSATest2(int rounds)
		{
			Random rand = new Random();
			byte[] val = new byte[64];

			byte[] pseudoPrime1 = {
				(byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
				(byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
				(byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
				(byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
				(byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
				(byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
				(byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
				(byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
				(byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
				(byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
				(byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
			};

			byte[] pseudoPrime2 = {
				(byte)0x99, (byte)0x98, (byte)0xCA, (byte)0xB8, (byte)0x5E, (byte)0xD7,
				(byte)0xE5, (byte)0xDC, (byte)0x28, (byte)0x5C, (byte)0x6F, (byte)0x0E,
				(byte)0x15, (byte)0x09, (byte)0x59, (byte)0x6E, (byte)0x84, (byte)0xF3,
				(byte)0x81, (byte)0xCD, (byte)0xDE, (byte)0x42, (byte)0xDC, (byte)0x93,
				(byte)0xC2, (byte)0x7A, (byte)0x62, (byte)0xAC, (byte)0x6C, (byte)0xAF,
				(byte)0xDE, (byte)0x74, (byte)0xE3, (byte)0xCB, (byte)0x60, (byte)0x20,
				(byte)0x38, (byte)0x9C, (byte)0x21, (byte)0xC3, (byte)0xDC, (byte)0xC8,
				(byte)0xA2, (byte)0x4D, (byte)0xC6, (byte)0x2A, (byte)0x35, (byte)0x7F,
				(byte)0xF3, (byte)0xA9, (byte)0xE8, (byte)0x1D, (byte)0x7B, (byte)0x2C,
				(byte)0x78, (byte)0xFA, (byte)0xB8, (byte)0x02, (byte)0x55, (byte)0x80,
				(byte)0x9B, (byte)0xC2, (byte)0xA5, (byte)0xCB,
			};


			BigInteger bi_p = new BigInteger(pseudoPrime1);
			BigInteger bi_q = new BigInteger(pseudoPrime2);
			BigInteger bi_pq = (bi_p-1)*(bi_q-1);
			BigInteger bi_n = bi_p * bi_q;

			for (int count = 0; count < rounds; count++)
			{
				// generate private and public key
				BigInteger bi_e = bi_pq.genCoPrime(512, rand);
				BigInteger bi_d = bi_e.modInverse(bi_pq);

				Console.WriteLine("\ne =\n" + bi_e.ToString(10));
				Console.WriteLine("\nd =\n" + bi_d.ToString(10));
				Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

				// generate data of random length
				int t1 = 0;
				while (t1 == 0)
					t1 = (int)(rand.NextDouble() * 65);

				bool done = false;
				while (!done)
				{
					for (int i = 0; i < 64; i++)
					{
						if (i < t1)
							val[i] = (byte)(rand.NextDouble() * 256);
						else
							val[i] = 0;

						if (val[i] != 0)
							done = true;
					}
				}

				while (val[0] == 0)
					val[0] = (byte)(rand.NextDouble() * 256);

				Console.Write("Round = " + count);

				// encrypt and decrypt data
				BigInteger bi_data = new BigInteger(val, t1);
				BigInteger bi_encrypted = bi_data.modPow(bi_e, bi_n);
				BigInteger bi_decrypted = bi_encrypted.modPow(bi_d, bi_n);

				// compare
				if (bi_decrypted != bi_data)
				{
					Console.WriteLine("\nError at round " + count);
					Console.WriteLine(bi_data + "\n");
					return;
				}
				Console.WriteLine(" <PASSED>.");
			}

		}


		//***********************************************************************
		// Tests the correct implementation of sqrt() method.
		//***********************************************************************

		public static void SqrtTest(int rounds)
		{
			Random rand = new Random();
			for (int count = 0; count < rounds; count++)
			{
				// generate data of random length
				int t1 = 0;
				while (t1 == 0)
					t1 = (int)(rand.NextDouble() * 1024);

				Console.Write("Round = " + count);

				BigInteger a = new BigInteger();
				a.genRandomBits(t1, rand);

				BigInteger b = a.sqrt();
				BigInteger c = (b+1)*(b+1);

				// check that b is the largest integer such that b*b <= a
				if (c <= a)
				{
					Console.WriteLine("\nError at round " + count);
					Console.WriteLine(a + "\n");
					return;
				}
				Console.WriteLine(" <PASSED>.");
			}
		}



		public static void _Main(string[] args)
		{
			// Known problem -> these two pseudoprimes passes my implementation of
			// primality test but failed in JDK's isProbablePrime test.

			byte[] pseudoPrime1 = { (byte)0x00,
				(byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
				(byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
				(byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
				(byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
				(byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
				(byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
				(byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
				(byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
				(byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
				(byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
				(byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
			};

			byte[] pseudoPrime2 = { (byte)0x00,
				(byte)0x99, (byte)0x98, (byte)0xCA, (byte)0xB8, (byte)0x5E, (byte)0xD7,
				(byte)0xE5, (byte)0xDC, (byte)0x28, (byte)0x5C, (byte)0x6F, (byte)0x0E,
				(byte)0x15, (byte)0x09, (byte)0x59, (byte)0x6E, (byte)0x84, (byte)0xF3,
				(byte)0x81, (byte)0xCD, (byte)0xDE, (byte)0x42, (byte)0xDC, (byte)0x93,
				(byte)0xC2, (byte)0x7A, (byte)0x62, (byte)0xAC, (byte)0x6C, (byte)0xAF,
				(byte)0xDE, (byte)0x74, (byte)0xE3, (byte)0xCB, (byte)0x60, (byte)0x20,
				(byte)0x38, (byte)0x9C, (byte)0x21, (byte)0xC3, (byte)0xDC, (byte)0xC8,
				(byte)0xA2, (byte)0x4D, (byte)0xC6, (byte)0x2A, (byte)0x35, (byte)0x7F,
				(byte)0xF3, (byte)0xA9, (byte)0xE8, (byte)0x1D, (byte)0x7B, (byte)0x2C,
				(byte)0x78, (byte)0xFA, (byte)0xB8, (byte)0x02, (byte)0x55, (byte)0x80,
				(byte)0x9B, (byte)0xC2, (byte)0xA5, (byte)0xCB,
			};

			Console.WriteLine("List of primes < 2000\n---------------------");
			int limit = 100, count = 0;
			for (int i = 0; i < 2000; i++)
			{
				if (i >= limit)
				{
					Console.WriteLine();
					limit += 100;
				}

				BigInteger p = new BigInteger(-i);

				if (p.isProbablePrime())
				{
					Console.Write(i + ", ");
					count++;
				}
			}
			Console.WriteLine("\nCount = " + count);


			BigInteger bi1 = new BigInteger(pseudoPrime1);
			Console.WriteLine("\n\nPrimality testing for\n" + bi1.ToString() + "\n");
			Console.WriteLine("SolovayStrassenTest(5) = " + bi1.SolovayStrassenTest(5));
			Console.WriteLine("RabinMillerTest(5) = " + bi1.RabinMillerTest(5));
			Console.WriteLine("FermatLittleTest(5) = " + bi1.FermatLittleTest(5));
			Console.WriteLine("isProbablePrime() = " + bi1.isProbablePrime());

			Console.Write("\nGenerating 512-bits random pseudoprime. . .");
			Random rand = new Random();
			BigInteger prime = BigInteger.genPseudoPrime(512, 5, rand);
			Console.WriteLine("\n" + prime);

			//int dwStart = System.Environment.TickCount;
			//BigInteger.MulDivTest(100000);
			//BigInteger.RSATest(10);
			//BigInteger.RSATest2(10);
			//Console.WriteLine(System.Environment.TickCount - dwStart);

		}
	}
}

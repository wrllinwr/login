using System;

namespace SFL.data.plugin
{
	public class StringTokenizer
	{
		private string _str;
		
		private string _delimiters;
		
		private bool _returnDelimiters;
		
		private int _position;
		
		public StringTokenizer(string str) : this(str, " \t\n\r\f", false) {}
		
		public StringTokenizer(string str, string delimiters) : this(str, delimiters, false) {}
		
		public StringTokenizer(string str, string delimiters, bool returnDelimiters)
		{
			if (str != null)
			{
				this._str = str;
				this._delimiters = delimiters;
				this._returnDelimiters = returnDelimiters;
				this._position = 0;
				return;
			}
			
			throw new NullReferenceException();
		}
		
		public int CountTokens()
		{
			int num = 0;
			bool flag = false;
			int i = this._position;
			int length = this._str.Length;
			
			while (i < length)
			{
				if (this._delimiters.IndexOf(this._str[i], 0) >= 0)
				{
					if (this._returnDelimiters)
					{
						num++;
					}
					
					if (flag)
					{
						num++;
						flag = false;
					}
					
				}
				else
				{
					flag = true;
				}
				
				i++;
			}
			
			if (flag)
			{
				num++;
			}
			
			return num;
		}
		
		public bool HasMoreElements()
		{
			return this.HasMoreTokens();
		}
		
		public bool HasMoreTokens()
		{
			int length = this._str.Length;
			bool result;
			if (this._position < length)
			{
				if (this._returnDelimiters)
				{
					result = true;
					return result;
				}
				
				for (int i = this._position; i < length; i++)
				{
					if (this._delimiters.IndexOf(this._str[i], 0) == -1)
					{
						result = true;
						return result;
					}
				}
				
			}
			
			result = false;
			return result;
		}
		
		public object NextElement()
		{
			return this.NextToken();
		}
		
		public string NextToken()
		{
			int num = this._position;
			int length = this._str.Length;
			if (num < length)
			{
				string result;
				if (this._returnDelimiters)
				{
					if (this._delimiters.IndexOf(this._str[this._position], 0) >= 0)
					{
						result = string.Concat(this._str[this._position++]);
					}
					else
					{
						this._position++;
						while (this._position < length)
						{
							if (this._delimiters.IndexOf(this._str[this._position], 0) >= 0)
							{
								result = this._str.Substring(num, this._position - num);
								return result;
							}
							this._position++;
						}
						
						result = this._str.Substring(num);
					}
				}
				else
				{
					while (num < length && this._delimiters.IndexOf(this._str[num], 0) >= 0)
					{
						num++;
					}
					
					this._position = num;
					
					if (num >= length)
					{
						goto IL_1B9;
					}
					
					this._position++;
					
					while (this._position < length)
					{
						if (this._delimiters.IndexOf(this._str[this._position], 0) >= 0)
						{
							result = this._str.Substring(num, this._position - num);
							return result;
						}
						this._position++;
					}
					
					result = this._str.Substring(num);
				}
				
				return result;
			}
			
			IL_1B9:
			throw new Exception();
		}
		
		public string NextToken(string delims)
		{
			this._delimiters = delims;
			return this.NextToken();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SFL
{
    public class Memory
    {
        public IntPtr Hwnd;
        
        public IntPtr Process;
        
        public int ProcessID;

        protected void Open(IntPtr Hwnd)
        {
            this.Hwnd = Hwnd;
            Win32API.GetWindowThreadProcessId(Hwnd, out ProcessID);
            Process = Win32API.OpenProcess(Win32API.ProcessAccessFlags.All, 0, ProcessID);
        }

        protected void Close()
        {
            Win32API.CloseHandle(Process);
            Process = IntPtr.Zero;
        }

        public byte[] ReadMemoryByteArray(int Address, int Count)
        {
            byte[] buff = new byte[Count];
            Win32API.ReadProcessMemory(Process, Address, buff, Count, 0);
            return buff;
        }

        public byte ReadMemoryByte(int Address)
        {
            byte[] buff = new byte[1];
            Win32API.ReadProcessMemory(Process, Address, buff, 1, 0);
            return buff[0];
        }

        public short ReadMemoryShort(int Address)
        {
            short[] buff = new short[1];
            Win32API.ReadProcessMemory(Process, Address, buff, 2, 0);
            return buff[0];
        }

        public int ReadMemoryInteger(int Address)
        {
            int[] buff = new int[1];
            Win32API.ReadProcessMemory(Process, Address, buff, 4, 0);
            return buff[0];
        }

        public string ReadMemoryString(int Address, int Size)
        {
            byte[] buff = new byte[Size];
            string retString;
            Win32API.ReadProcessMemory(Process, Address, buff, Size, 0);
            retString = System.Text.Encoding.Default.GetString(buff) + "\0";
            retString = retString.Substring(0, retString.IndexOf("\0"));
            return retString;
        }

        public void WriteMemoryByteArray(int Address, byte[] Value)
        {
            Win32API.WriteProcessMemory(this.Process, Address, Value, Value.Length, 0);
        }

        public void WriteMemoryByteArray(int Address, byte[] Value, int Size)
        {
            Win32API.WriteProcessMemory(this.Process, Address, Value, Size, 0);
        }

        public void WriteMemoryByte(int Address, byte Value)
        {
            Win32API.WriteProcessMemory(Process, Address, ref Value, 1, 0);
        }

        public void WriteMemoryShort(int Address, short Value)
        {
            Win32API.WriteProcessMemory(Process, Address, ref Value, 2, 0);
        }

        public void WriteMemoryInteger(int Address, int Value)
        {
            Win32API.WriteProcessMemory(Process, Address, ref Value, 4, 0);
        }

        public void WriteMemoryString(int Address, string Value)
        {
            byte[] buff = System.Text.Encoding.Default.GetBytes(Value);
            Win32API.WriteProcessMemory(Process, Address, buff, buff.Length, 0);
        }
    }
}

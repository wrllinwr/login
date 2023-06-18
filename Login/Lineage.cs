using System;
using System.Collections.Generic;
using System.Text;

namespace SFL
{
    class Lineage : Memory
    {
        // private int _version; // 版本號
        private int _timeLimitAddress; // 時間限制
        private int _portAddress; // 端口地址
        private int _multiClientAddress; // 多開地址
        
        private int _dayAddress; // 全白天
        private int _pumperAddress; // 抽水
        
        private int _listsprFixAddress; // 解除list驗證
        private int _listsprBeginAddress; // list起始地址
        private int _listsprEndAddress; // list結束地址
        private int _listsprCountAddress; // list總數
        
        
        public Lineage()
        {
            // _version = 712060102;
            _timeLimitAddress = 0x004A6AFD;
            _portAddress = 0x008FB234;
            _multiClientAddress = 0x004A63BE;
            
            _dayAddress = 0x004B450A;
            _pumperAddress = 0x004B4DCD;
            
            _listsprFixAddress = 0x005315FC;
            _listsprBeginAddress = 0x00658580;
            _listsprEndAddress = 0x008FB075;
            _listsprCountAddress = 0x008FB0F8;          
        }

        public new void Open(IntPtr Hwnd)
        {
            base.Open(Hwnd);
        }

        public new void Close()
        {
            base.Close();
        }

        public void OpenById(int ProcessID)
        {
            Process = Win32API.OpenProcess(Win32API.ProcessAccessFlags.All, 0, ProcessID);
        }
        
        public bool ChangeSetInStart(int port)
        {
            if (ReadMemoryByte(_timeLimitAddress) != 0x0f)
            {
                return false;               
            }
            else
            {
            	byte[] timeData = { 0xE9, 0x98, 0x00 }; // 004a6afd  的0x0f 0x85 0x97 修改成0xE9 0x98 0x00
            	byte[] multiData = { 0xEB }; // 004A63BE：74改EB
                WriteMemoryByteArray(_timeLimitAddress, timeData); // 修改時間限制
                WriteMemoryInteger(_portAddress, port); // 修改端口                           
                WriteMemoryByteArray(_multiClientAddress, multiData); // 客戶端多開   
                return true;
            }
        }
        
        public void ChangeSetInRun()
        {
        	byte[] dayData = { 0x90, 0xE9 }; // 004B450A：0F8D 改 90E9
            WriteMemoryByteArray(_dayAddress, dayData); // 全白天
            
            byte pumpData = 0xEB; // 004B4DCD：74 改 EB
            WriteMemoryByte(_pumperAddress, pumpData); // 抽水      
        }

        public void ChangeListsprSet()
        {
        	byte sprfixData = 0xEB; // 0x005315FC 74改EB
            WriteMemoryByte(_listsprFixAddress, sprfixData); // list解除驗證
            
            byte[] sprcountData = { 0xA9, 0xA9 }; // 0x008FB0F8 23 28 改成 FF FF
            WriteMemoryByteArray(_listsprCountAddress, sprcountData); // list數量         
        }

        public void WriteListspr(byte[] Data)
        {
            WriteMemoryByteArray(_listsprBeginAddress, Data);
        }

        public int GetListSprSize()
        {
            int ListSprSize = _listsprEndAddress - _listsprBeginAddress;
            return ListSprSize;
        }

    }
}

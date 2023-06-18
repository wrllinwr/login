using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace SFL
{
    class PingClass
    {
        public bool Test(string ServerIP, int ServerPort, int Timeout)
        {
            TcpClient myTcpClient = new TcpClient();
            IAsyncResult MyResult = myTcpClient.BeginConnect(ServerIP, ServerPort, new AsyncCallback(ConnectCallback), myTcpClient);
            MyResult.AsyncWaitHandle.WaitOne(Timeout, true);
            if (MyResult.IsCompleted && myTcpClient.Connected)
            {
                System.Threading.Thread.Sleep(500);
                myTcpClient.Close();
                return true;
            }
            else
            {
                System.Threading.Thread.Sleep(500);
                myTcpClient.Close();
                return false;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;
                client.EndConnect(ar);
            }
            catch { }
        }
    }
}

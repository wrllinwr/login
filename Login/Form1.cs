using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using SFL.Properties;
using SFL.data.crypt;
using SFL.data.packet;
using SFL.data.plugin;

namespace SFL
{
    public partial class Form1 : Form
    {
    	private Crc32 _crc = new Crc32();
        private ArrayList _pidlist = new ArrayList();
        private int _msgShellHook = Win32API.RegisterWindowMessage("SHELLHOOK");
        private ServerInfo[] _serverInfo = new ServerInfo[20];
        private Socket _socketListener;
        private Socket _socketToC;
        private Socket _socketToS;
        private Thread _threadToC;
        private Thread _threadToS;
        private Thread _threadBot;
        private Process _process;
        private byte _xorByte;
        private static bool _isInitKey = false; // 獲取seed標記
        private static PacketServer _packetServer = new PacketServer();// 服務端回應
        private static PacketClient _packetClient = new PacketClient();// 客戶端回應
        private int _flag = -1;
        private int _bridgePort = 3000; // 橋接端口
        
        public Form1()
        {
            InitializeComponent();
        }

        private struct ServerInfo
        {
            public string SERVER_NAME;
            public string SERVER_IP;
            public int SERVER_PORT;
            public int LIST_SPR;
            public string SPR_FILENAME;
            public uint SPR_CRC;
            public string FILE_NAME;
            public ulong RSA_D;
            public ulong RSA_N;
            public bool CHECK_A;
            public bool CHECK_B;
            public bool CHECK_C;
        }

        #region 移動視窗
        private Point prevLeftClick;
        private bool isFirst = true;
        private bool toBlock = true;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isFirst == true)
                {
                    prevLeftClick = new Point(e.X, e.Y);
                    isFirst = false;
                }
                else
                {
                    if (toBlock == false)
                        this.Location = new Point(this.Location.X + e.X - prevLeftClick.X, this.Location.Y + e.Y - prevLeftClick.Y);
                    prevLeftClick = new Point(e.X, e.Y);
                    toBlock = !toBlock;
                }
            }
            else
            {
                isFirst = true;
            }

            if (e.X > 666 && e.X < 684 && e.Y > 6 && e.Y < 25)
            {
                this.Cursor = Cursors.Hand;
            }
            else if (e.X > 398 && e.X < 495 && e.Y > 412 && e.Y < 447)
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == _msgShellHook)
            {
                switch (m.WParam.ToInt32())
                {
                    case Win32API.HSHELL_WINDOWCREATED:
                        IntPtr hWmd = (IntPtr)m.LParam.ToInt32();
                        if (Win32API.GetClassName(hWmd) == "Lineage")
                        {
                        	byte[] buffer;
                            Crc32 crc;
                            Lineage lin = new Lineage();
                            lin.Open(hWmd);
                            // 可添加一些內存修改的活
                            lin.ChangeSetInRun();
                            if ((this._serverInfo[this._flag].LIST_SPR == 2) 
                                && File.Exists(this._serverInfo[this._flag].SPR_FILENAME))
                            {
                                buffer = File.ReadAllBytes(this._serverInfo[this._flag].SPR_FILENAME);
                                crc = new Crc32();
                                crc.CrcAdd(buffer, buffer.Length);
                                if ((crc.CrcGet() ^ 07386295) == this._serverInfo[this._flag].SPR_CRC) // 變檔CRC校驗值
                                {
                                    lin.ChangeListsprSet();
                                    lin.WriteListspr(buffer);
                                }
                            }
                            
                            lin.Close();
                            Win32API.DeregisterShellHookWindow(this.Handle);
                        }
                        break;
                }
            }
            base.DefWndProc(ref m);
        }

        private bool CheckBOT1()
        {
            if (_serverInfo[_flag].CHECK_A)
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (_pidlist.IndexOf(p.Id) >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        _pidlist.Add(p.Id);
                    }

                    try
                    {
                        uint CrcNum = 0;
                        ArrayList fNameList = AntiBotData.GetInstance().getFName();
                        ArrayList pNameList = AntiBotData.GetInstance().getPName();
                        int fNameCount = fNameList.Count;
                        int pNameCount = pNameList.Count;
                        
                        foreach (ProcessModule m in p.Modules)
                        {
                            _crc.Crc(m.ModuleName.ToLower());
                            CrcNum = _crc.CrcGet();                          
                            for (int i = 0; i < fNameCount; i++)
                            {
                            	if (((uint) (fNameList[i])) == CrcNum)
                                {
                                    return true;
                                }
                            }
                        }

                        _crc.Crc(p.Modules[0].FileVersionInfo.ProductName);
                        CrcNum = _crc.CrcGet();
                        for (int i = 0; i < pNameCount; i++)
                        {
                            if (((uint) (pNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }

                        _crc.Crc(p.Modules[0].FileVersionInfo.InternalName);
                        CrcNum = _crc.CrcGet();
                        for (int i = 0; i < pNameCount; i++)
                        {
                            if (((uint) (pNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }

                        _crc.Crc(p.Modules[0].FileVersionInfo.FileDescription);
                        CrcNum = _crc.CrcGet();
                        for (int i = 0; i < pNameCount; i++)
                        {
                            if (((uint) (pNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }
                        _crc.Crc(p.Modules[0].FileVersionInfo.LegalCopyright);
                        CrcNum = _crc.CrcGet();
                        for (int i = 0; i < pNameCount; i++)
                        {
                            if (((uint) (pNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }
                        _crc.Crc(p.Modules[0].FileVersionInfo.FileVersion);
                        CrcNum = _crc.CrcGet();
                        for (int i = 0; i < pNameCount; i++)
                        {
                            if (((uint) (pNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }
                        _crc.Crc(p.Modules[0].FileVersionInfo.Language);
                        CrcNum = _crc.CrcGet();
                        for (int i = 0; i < pNameCount; i++)
                        {
                            if (((uint) (pNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }
                    }
                    catch { }
                }

                IntPtr dlgHandle = Win32API.GetWindow(Win32API.GetDesktopWindow(), Win32API.GetWindow_Cmd.GW_CHILD);
                while (dlgHandle != IntPtr.Zero)
                {
                    string title = "";
                    string className = "";

                    title = Win32API.GetWindowText(dlgHandle);
                    IDictionary<string, ArrayList> wTitle = AntiBotData.GetInstance().getWTitle();
                    int wtLength = wTitle.Count;
                    
                    foreach (string key in wTitle.Keys)
                    {
                    	if (title.IndexOf(key) >= 0)
                    	{
                    		className = Win32API.GetClassName(dlgHandle);
                    		foreach (string val in wTitle[key])
                    		{
                    		    if (className == val || val.Length == 0)
                                {
                                    return true;
                                }
                    		}
                    	}
                    }
                                        
                    /*for (int i = 0; i < (wtLength / 2); i++)
                    {
                        if (Title.IndexOf(wTitle[i, 0]) >= 0)
                        {
                            Classname = Win32API.GetClassName(dlgHandle);
                            if (Classname == WTitle[i, 1] || WTitle[i, 1].Length == 0)
                            {
                                return true;
                            }
                        }
                    }*/
                    dlgHandle = Win32API.GetWindow(dlgHandle, Win32API.GetWindow_Cmd.GW_HWNDNEXT);
                }
            }

            if (_serverInfo[_flag].CHECK_B)
            {
                bool flag = false;
                Mutex mutex = new Mutex(true, "LinHelper2", out flag);
                if (flag)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                }
                else
                {
                    mutex.Close();
                    return true;
                }

                IntPtr hWnd = Win32API.FindWindow("LH", null);
                if (hWnd != IntPtr.Zero)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckBOT2()
        {
            if (_serverInfo[_flag].CHECK_A)
            {
                try
                {
                    Process p = Process.GetProcessById(_process.Id);
                    foreach (ProcessModule m in p.Modules)
                    {
                        _crc.Crc(m.ModuleName.ToLower());
                        uint CrcNum = _crc.CrcGet();
                        ArrayList fNameList = AntiBotData.GetInstance().getFName();
                        for (int i = 0; i < fNameList.Count; i++)
                        {
                        	if (((uint) (fNameList[i])) == CrcNum)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch { }
            }
            return false;
        }

        private bool CheckClient()
        {
			try
			{
				int linProcNum = 0;
				IntPtr dlgHandle = Win32API.GetWindow(Win32API.GetDesktopWindow(), Win32API.GetWindow_Cmd.GW_CHILD);
				while (dlgHandle != IntPtr.Zero)
				{
					string title = Win32API.GetWindowText(dlgHandle);
					string className = Win32API.GetClassName(dlgHandle);
					if (title.Equals("Lineage Windows Client (12011702)") && className.Equals("Lineage"))
					{
						++ linProcNum;
					}
					
					if (linProcNum >= 3)
					{
						return true;
					}

					dlgHandle = Win32API.GetWindow(dlgHandle, Win32API.GetWindow_Cmd.GW_HWNDNEXT);
				}
			}
			catch
			{
				return true;
			}
			
			return false;
        }
        
        private void CheckBOT()
        {
        	bool isCheat = false; // 是否檢測到開啟作弊的標記
        	bool isClientExceed = false; // 是否檢測到多開超出限制的標記
        	try
        	{
        		while (!_process.HasExited)
        		{
        			if (CheckBOT1() || CheckBOT2())
        			{
        				isCheat = true;
        				break;
        			}
        			
        			if (CheckClient())
        			{
        				isClientExceed = true;
        				break;
        			}
        			
        			Thread.Sleep(2000);
        		}
        	}
        	catch(Exception ex)
        	{
                // 待处理
                Debug.WriteLine(ex.ToString());
            }
        	finally
        	{
        		if (isCheat || isClientExceed)
        		{
        			System.Timers.Timer quitTimer = new System.Timers.Timer(300);
        			quitTimer.Elapsed += new ElapsedEventHandler(timers_Elapsed);
        			quitTimer.AutoReset = true;
        			quitTimer.Enabled = true;
        			quitTimer.Start();
        			
        			if (isCheat)
        			{
        				MessageBox.Show("檢測到與登陸器衝突的程式，請關閉不必要的程式！", "天堂",
        				                MessageBoxButtons.OK, MessageBoxIcon.Error,
        				                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        			}
        			else if (isClientExceed)
        			{
        				MessageBox.Show("只允許同時開啟兩個客戶端！", "天堂");
        			}

        		}
        		else
        		{
                    Console.WriteLine("Form1.CheckBOT处调动KillGame" + " " + isCheat + " " + isClientExceed);
        			this.KillGame();
        		}
        	}
        }
        
        private static void timers_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Form1.timers_Elapsed调用KillGame");
        	Program.GetForm1().KillGame();
        }
        
        /// <summary>
		/// 關閉遊戲
		/// </summary>
        public void KillGame()
        {
            try
            {
            	if ((_process != null) && (!_process.HasExited))
                {
                	_process.Kill();
                }
            	
                _socketToC.Close();
                _socketToS.Close();
            }
            catch { }

            Console.WriteLine("Form1.KillGame 退出");
            Environment.Exit(0);
        }

        private void Run(ServerInfo si)
        {
            string FileName = Environment.CurrentDirectory + "\\" + si.FILE_NAME;
            // string FileName = "F:\\Lineage3.5TC\\" + si.FILE_NAME; // 調試用
            if (!System.IO.File.Exists(FileName))
            {
                MessageBox.Show("無法執行 " + si.FILE_NAME, " 請檢查文件路徑！");
                Application.Exit();
                return;
            }
            else if (CheckBOT1())
            {
            	MessageBox.Show("請關閉相關非法程序才能運行遊戲！");
                Application.Exit();
                return;
            }
            
            IPEndPoint LocalEP;
            Random r = new Random();
            _bridgePort = r.Next(10000, 50000); // 修改端口從10000開始，減小衝突幾率
            try
            {
                LocalEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _bridgePort); // 設定IP終結點，即固定目標IP和端口
                _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socketListener.Bind(LocalEP);
                _socketListener.Listen(1); // 只允許第一個和終結點握手的Socket進行連接(與EndAccept配合)
                _socketListener.BeginAccept(new AsyncCallback(ConnectCallback), _socketListener); // 異步監聽託管

                Win32API.RegisterShellHookWindow(this.Handle);
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} {1:d}", "2130706433", _bridgePort); // 打開lin文件時的後綴，端口和橋接端口一致
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = FileName;
                startInfo.UseShellExecute = false;
                startInfo.Arguments = sb.ToString();
                
                if (!Program.CheckLinProcess())
                {
                	System.Timers.Timer quitTimer = new System.Timers.Timer(300);
        			quitTimer.Elapsed += new ElapsedEventHandler(timers_Elapsed);
        			quitTimer.AutoReset = true;
        			quitTimer.Enabled = true;
        			quitTimer.Start();
        			
        			MessageBox.Show("只允許同時開啟兩個客戶端！", "天堂");
                }
                
                _process = Process.Start(startInfo); // 以指定參數開啟核心
                
                // 開始執行內存修改
                Lineage Lin = new Lineage();
                Lin.OpenById(_process.Id);
                while(true)
                {
                	if (!Lin.ChangeSetInStart(_bridgePort))
                	{
                		Thread.Sleep(50);
                	}
                	else
                	{
                		break;
                	}
                }
                Lin.Close();

                //_threadBot = new Thread(CheckBOT);
                //_threadBot.IsBackground = true;
                //_threadBot.Start();
            }
            catch (Win32Exception ex)
            {
                int errorCode = ex.ErrorCode;
                string errorMessage = ex.Message;
                Console.WriteLine($"Win32Exception occurred. ErrorCode: {errorCode}, Message: {errorMessage}");
                Console.WriteLine("Form1.Run 退出");
                Environment.Exit(0);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _socketToC = _socketListener.EndAccept(ar); // 當遊戲Socket和終結點握手后，關閉監聽，并傳遞該Socket值，此時橋接建立完畢
                _socketToS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // 讀取遠程服務器端口的數據
                _socketToS.Connect(_serverInfo[_flag].SERVER_IP, _serverInfo[_flag].SERVER_PORT);
                _threadToC = new Thread(Proc1);
                _threadToC.IsBackground = true;
                _threadToC.Start();

                _threadToS = new Thread(Proc2);
                _threadToS.IsBackground = true;
                _threadToS.Start();
                _socketListener.Close();
            }
            catch
            {
                Console.WriteLine("Fomr1.ConnectCallback 退出");
                Environment.Exit(0);
            }
        }
        
        // 接受服務器端來的封包並解密，發送解密後的封包給遊戲
        private void Proc1()
        {
        	// 獲取封包加密的密鑰
            if (_serverInfo[_flag].RSA_N > 0)
            {
            	byte[] data = new byte[4];
                _socketToS.Receive(data, 4, SocketFlags.None);
                int xorByteData = BitConverter.ToInt32(data, 0);
                BigInteger big = xorByteData;
                long key = big.modPow(_serverInfo[_flag].RSA_D ^ 84425881, _serverInfo[_flag].RSA_N ^ 62952021).LongValue();
                _xorByte = (byte)key;
            }

            // 開始讀包循環
            while (true)
            {
                try
                {
                	byte[] begindata = new byte[2];
                	_socketToS.Receive(begindata, 2, SocketFlags.None);
                	int hiByte = begindata[0];
				    int loByte = begindata[1];
				    
				    if (loByte < 0)
				    {
					    break;
				    }
				        
				    int dataLength = ((loByte << 8) + hiByte) - 2;	
				    byte[] remaindata = new byte[dataLength];	
				    int readSize = 0;
	
				    for (int i = 0; (i != -1) && (readSize < dataLength); readSize += i)
				    {
				    	i = _socketToS.Receive(remaindata, readSize, (dataLength - readSize), SocketFlags.None);
				    }
	
				    if (readSize != dataLength)
				    {
					    break;
				    }
				        
				    if (!_isInitKey) // 取得初始Key值並加載封包處理類
                    {
                        int seed = 0;
                        
                        seed |= (int) (remaindata[1] << 0 & 0xff);
                        seed |= (int) (remaindata[2] << 8 & 0xff00);
                        seed |= (int) (remaindata[3] << 16 & 0xff0000);
                        seed |= (int) (remaindata[4] << 24 & 0xff000000);

					    LinEncrypt.InitKeys(seed);
					    EncryptForS.InitKeys(seed);
					    EncryptForC.InitKeys(seed);
				        OpLoad.Load(seed);
				        _isInitKey = true;
				    }
				    else // 解包並判定
				    {
				    	byte[] decrypt = new byte[dataLength];
				    	System.Array.Copy(remaindata, 0, decrypt, 0, dataLength);

				    	lock (this)
				    	{
                            decrypt = EncryptForS.Decrypt(decrypt, dataLength);
					    }

				    	_packetServer.HandlePacket(decrypt);
				    }
				    
	                int pcklength = dataLength + 2;
				    byte[] pck = new byte[pcklength];
				    pck[0] = begindata[0];
				    pck[1] = begindata[1];
				    System.Array.Copy(remaindata, 0, pck, 2, dataLength);
                    _socketToC.Send(pck, 0, pcklength, SocketFlags.None); // 送出封包給客戶端
                }
                catch
                {
                    break;
                }
            }
        }

        // 接收遊戲端來的封包，並加密，發給服務端
        private void Proc2()
        {
            while (true)
            {
                try
                {
                	byte[] begindata = new byte[2];
                	_socketToC.Receive(begindata, 2, SocketFlags.None);
                	int hiByte = begindata[0];
				    int loByte = begindata[1];
				    if (loByte < 0)
				    {
					    break;
				    }
				        
				    int dataLength = ((loByte << 8) + hiByte) - 2;	
				    byte[] remaindata = new byte[dataLength];	
				    int readSize = 0;
	
				    for (int i = 0; (i != -1) && (readSize < dataLength); readSize += i)
				    {
				    	i = _socketToC.Receive(remaindata, readSize, (dataLength - readSize), SocketFlags.None);
				    }
	
				    if (readSize != dataLength)
				    {
					    break;
				    }
                	
				    byte[] decrypt = new byte[dataLength];
				    System.Array.Copy(remaindata, 0, decrypt, 0, dataLength);

				    lock (this)
				    {
                        decrypt = EncryptForC.Decrypt(decrypt, dataLength);
					}

				    // 當封包狀態正常則繼續，否則直接退出處理循環(丟棄封包會導致客戶端不斷發送莫名封包)
				    // bool result = _packetClient.HandlePacket(decrypt);
				    bool result = true;
				    if (result)
				    {
				        int pcklength = dataLength + 2;
				        byte[] pck = new byte[pcklength];
				        pck[0] = (byte) (pcklength & 0xff);
				        pck[1] = (byte) (pcklength >> 8 & 0xff);
				        System.Array.Copy(remaindata, 0, pck, 2, dataLength);
				    
                        if (_serverInfo[_flag].RSA_N > 0)
                        {
                            for (int i = 0; i < pcklength; i++)
                            {
                                pck[i] = (byte)(pck[i] ^ _xorByte);
                            }
                        }
                        
                        _socketToS.Send(pck, 0, pcklength, SocketFlags.None);
				    }
				    else
				    {
				    	break;
				    }
                }
                catch
                {
                    break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                bool DataOK = false;
                string SeverData = Win32API.GetINI("Server" + (i + 1), "ServerData", "", Program.GetIniFile());
                if (SeverData.Length > 10)
                {
                    try
                    {
                        byte[] buff = Convert.FromBase64String(SeverData);
                        int sum = 0;
						int index = buff.Length - 1;
                        while (index > 0)
                        {
                            buff[index] = (byte) (buff[index] ^ buff[index - 1]);
                            sum += buff[index];
                            index--;
                        }


                        byte seed = (byte)(sum % 256);
                        if (buff[0] == seed)
                        {
                        	String data = Encoding.UTF8.GetString(buff, 1, buff.Length - 1);
                            string[] arr = data.Split(new string[] { @"/@**@\" }, StringSplitOptions.None);
                            if (arr[0] == "2")
                            {
                                _serverInfo[i].SERVER_NAME = arr[1];
                                _serverInfo[i].FILE_NAME = arr[2];
                                _serverInfo[i].SERVER_IP = arr[3];
                                _serverInfo[i].SERVER_PORT = Convert.ToInt32(arr[4]);
                                _serverInfo[i].RSA_D = Convert.ToUInt64(arr[5]);
                                _serverInfo[i].RSA_N = Convert.ToUInt64(arr[6]);
                                if (arr[7].Substring(0, 1) == "1") _serverInfo[i].CHECK_A = true;
                                if (arr[7].Substring(1, 1) == "1") _serverInfo[i].CHECK_B = true;
                                if (arr[7].Substring(2, 1) == "1") _serverInfo[i].CHECK_C = true;
                                _serverInfo[i].LIST_SPR = Convert.ToInt32(arr[8]);
                                _serverInfo[i].SPR_FILENAME = arr[9];
                                _serverInfo[i].SPR_CRC = Convert.ToUInt32(arr[10]);
                                DataOK = true;
                            }
                            data = "";
                        }
                        for (int j = 0; j < buff.Length; j++)
                        {
                            buff[j] = 0;
                        }
                    }
                    catch { }
                }


                if (DataOK && _serverInfo[i].SERVER_IP.Length > 5)
                {
                	// _serverInfo[i].SERVER_NAME = Win32API.GetINI("Server" + (i + 1), "ServerName", "", Program.GetIniFile());
                    // _serverInfo[i].FILE_NAME = Win32API.GetINI("Server" + (i + 1), "FileName", "lin.bin.exe", Program.GetIniFile());
                    ((Label)this.Controls.Find("label" + (i + 1).ToString(), true)[0]).Text = _serverInfo[i].SERVER_NAME;
                    ((PictureBox)this.Controls.Find("pictureBox" + (i + 1).ToString(), true)[0]).Visible = true;
                    Thread t = new Thread(Ping); //連接指示燈
                    t.Start(i);
                }
                else
                {
                    _serverInfo[i].SERVER_NAME = "";
                    _serverInfo[i].FILE_NAME = "";
                    _serverInfo[i].SERVER_IP = "";
                    _serverInfo[i].SERVER_PORT = 2000;
                    _serverInfo[i].RSA_D = 0L;
                    _serverInfo[i].RSA_N = 0L;
                    _serverInfo[i].CHECK_A = false;
                    _serverInfo[i].CHECK_B = false;
                    _serverInfo[i].CHECK_C = false;
                }
            }


            Bitmap bmp = SFL.Properties.Resources.Serverlist;
            if (Program.IsSkinEnable() && File.Exists(Program.GetServerlist()))
            {
            	bmp = new Bitmap(Program.GetServerlist());
            }
            BitmapRegion BitmapRegion = new BitmapRegion();
            BitmapRegion.CreateControlRegion(this, bmp);

            if (Program.IsSkinEnable() && File.Exists(Program.GetButton()))
            {
                Bitmap btnbmp = new Bitmap(Program.GetButton());
                label1.Image = btnbmp;
                label2.Image = btnbmp;
                label3.Image = btnbmp;
                label4.Image = btnbmp;
                label5.Image = btnbmp;
                label6.Image = btnbmp;
                label7.Image = btnbmp;
                label8.Image = btnbmp;
            }

            int ret = 0;
            Win32API.RtlAdjustPrivilege(20, true, false, ref ret);
            Win32API.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        private void Ping(object obj)
        {
            int i = (int)obj;
            try
            {
                PingClass ping = new PingClass();
                if (ping.Test(_serverInfo[i].SERVER_IP, _serverInfo[i].SERVER_PORT, 5000))
                {
                    ((PictureBox)this.Controls.Find("pictureBox" + (i + 1).ToString(), true)[0]).Image = SFL.Properties.Resources.Ball_Green;
                }
                else
                {
                    ((PictureBox)this.Controls.Find("pictureBox" + (i + 1).ToString(), true)[0]).Image = SFL.Properties.Resources.Ball_Red;
                }
            }
            catch { }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.X > 666 && e.X < 684 && e.Y > 6 && e.Y < 25)
            {
                Application.Exit();
            }
            else if (e.Button == MouseButtons.Left && e.X > 398 && e.X < 495 && e.Y > 412 && e.Y < 447)
            {
                Application.Exit();
            }
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            Point p = new Point();
            p.X = ((Label)sender).Location.X + 1;
            p.Y = ((Label)sender).Location.Y + 1;
            ((Label)sender).Location = p;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            Point p = new Point();
            p.X = ((Label)sender).Location.X - 1;
            p.Y = ((Label)sender).Location.Y - 1;
            ((Label)sender).Location = p;
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!(e.Button == MouseButtons.Left)) return;

            if ((Label)sender == label1)
                _flag = 0;
            else if ((Label)sender == label2)
                _flag = 1;
            else if ((Label)sender == label3)
                _flag = 2;
            else if ((Label)sender == label4)
                _flag = 3;
            else if ((Label)sender == label5)
                _flag = 4;
            else if ((Label)sender == label6)
                _flag = 5;
            else if ((Label)sender == label7)
                _flag = 6;
            else if ((Label)sender == label8)
                _flag = 7;
            else
                return;

            if (_serverInfo[_flag].SERVER_IP.Length > 5)
            {
                this.Hide();
                Run(_serverInfo[_flag]);
                Win32API.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

    }
}

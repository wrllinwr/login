using SFL.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Zip;
using SFL.data.plugin;
    
namespace SFL
{
    public class Form2 : Form
    {
        private int _blueProcessValue = 0;
        
        private int _countdownloadFiles = 0;
        
        private List<string> _downloadFileslist = new List<string>();
        
        private int _redProcessValue = 0;
        
        private BackgroundWorker _backgroundWorker1;
        
        private PictureBox _blueProcessBar;
        
        private IContainer _components = null;
        
        private Process _eatPro = new Process();
        
        private bool _isFirst = true;
        
        private Label _labVerInfo;
        
        private PictureBox _pictureBox1;
        
        private PictureBox _pictureBox2;
        
        private PictureBox _pictureBox10;
        
        private Point _prevLeftClick;
        
        private PictureBox _redProcessBar;
        
        private bool _toBlock = true;
        
        private WebBrowser _webBrowser1;

        public Form2(string url)
        {
            this.InitializeComponent();
            if (url != "")
            {
                this._webBrowser1.Navigate(url);
            }
            else
            {
                this._webBrowser1.Navigate("about:blank");
                this._webBrowser1.Document.BackColor = Color.Black;
            }
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this._backgroundWorker1.ReportProgress(0, "Start");
            foreach (UpdateInfo info in Ini.GetUpdateFiles())
            {
                this._countdownloadFiles += info.Files.Count;
            }
            foreach (UpdateInfo info in Ini.GetUpdateFiles())
            {
                foreach (string str in info.Files)
                {
                    using (BabyWebClient client = new BabyWebClient())
                    {
                    	if (str.Contains("npklogin"))
                        {
                        	client.CurrentFile = str;
                        	client.TargetFile = str;
                        	File.Delete(Application.StartupPath + "//" + client.TargetFile);
                        	client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.webc_DownloadFileCompleted);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.webc_DownloadProgressChanged);
                            client.DownloadFileAsync(new Uri(info.Folder + str), (Application.StartupPath + "//" + client.TargetFile));
                        }
                    	else
                    	{                    	
                            client.CurrentFile = str;
                            client.TargetFile = info.UpdateVersion.ToString() + str;
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.webc_DownloadFileCompleted);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.webc_DownloadProgressChanged);
                            client.DownloadFileAsync(new Uri(info.Folder + str), (Application.StartupPath + "//UpdateTemp//" + client.TargetFile));
                    	}
                    	
                        while (client.IsBusy)
                        {
                            Thread.Sleep(0x3e8);
                        }
                    }
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState.ToString() == "Start")
            {
                this._pictureBox1.Visible = false;
            }
            this._pictureBox1.Enabled = false;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	Boolean isSucess = false; // 是否吃檔成功
            this.BlueProcessValue = 100;
            this.RedProcessValue = 100;
            // Settings.Default.LastUpdateTime = Ini.GetLastGetUpdateTime();
            // Settings.Default.Save();
            ProcessStartInfo info2 = new ProcessStartInfo();
            info2.FileName = Application.StartupPath + @"\lineag.exe"; // 这是eat.exe
            // info2.FileName = Application.StartupPath + @"\S3DS2Y.bin";
            ProcessStartInfo startInfo = info2;
            try
            {
                this._eatPro = Process.Start(startInfo);
                this.Hide();
                isSucess = true;
            }
            catch (Win32Exception exception)
            {
                Console.WriteLine("出錯了，請重新嘗試！{0}", exception.Message);
            }
            while (!this._eatPro.HasExited)
            {
                Thread.Sleep(500);
            }
            
            this.Show();
            
            if (isSucess)
            {	
            	string folder1str = Directory.GetCurrentDirectory() + "\\sprite\\";
            	string folder2str = Directory.GetCurrentDirectory() + "\\text\\";
            	string folder3str = Directory.GetCurrentDirectory() + "\\surf\\";
            	string folder4str = Directory.GetCurrentDirectory() + "\\tile\\";
            	DirectoryInfo folder1 = new DirectoryInfo(folder1str);
            	DirectoryInfo folder2 = new DirectoryInfo(folder2str);
            	DirectoryInfo folder3 = new DirectoryInfo(folder3str);
            	DirectoryInfo folder4 = new DirectoryInfo(folder4str);
            	int folder1files = folder1.GetFiles().Length + folder1.GetDirectories().Length;
            	int folder2files = folder2.GetFiles().Length + folder2.GetDirectories().Length;
            	int folder3files = folder3.GetFiles().Length + folder3.GetDirectories().Length;
            	int folder4files = folder4.GetFiles().Length + folder4.GetDirectories().Length;
            		
            	if (folder1files == 0 && folder2files == 0 && folder3files == 0 && folder4files == 0)
            	{
            		string targetFile = Directory.GetCurrentDirectory() + "\\" + "npklogin4.sys";
            		foreach (int updateVer in Ini.GetTmpUpdateVer())
            		{
            			StreamWriter sw;
            			FileStream fs = null;
            			// 定義寫入類型
            			if (!File.Exists(targetFile))
            			{
            				fs = new FileStream(targetFile, FileMode.Create);
            				sw = new StreamWriter(fs);
            			}
            			else
            			{
            				sw = File.AppendText(targetFile);
            			}

            			try
            			{
            				sw.WriteLine(updateVer.ToString()); // 逐行寫入
            				sw.Flush(); // 輸出
            			}
            			catch
            			{
            				continue;
            			}
            			finally
            			{
            				sw.Close(); // 關閉輸出流
            				if (fs != null)
            				{
            					fs.Close(); //關閉文件流
            				}
            			}
            		}
            	}
            }
            
            string fileName = Directory.GetCurrentDirectory() + "\\npklogin0.sys";
            if (File.Exists(fileName))
            {
            	this._labVerInfo.Text = "Now Update Main Login File. Please Wait...";
            	Thread.Sleep(500);
            	//Util.KillSelfThenRun();
            }
            
            this._pictureBox1.Visible = true;
            this._pictureBox1.Enabled = true;
            this._labVerInfo.Text = "Press 'Start' to play Lineage.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this._components != null))
            {
                this._components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
		/// 載入遠程更新信息
		/// </summary>
		///		
		private static void LoadUpdate()
		{
			try
			{
				Ini.DownloadIni(Setting.WebUrl + "update.txt", "verinfo.dat");
				Ini.SetUpdateFiles("verinfo.dat");
				if (!Directory.Exists("UpdateTemp"))
				{
					Directory.CreateDirectory("UpdateTemp");
				}
			}
			catch
			{
				MessageBox.Show("遊戲更新列表讀取異常，請聯繫管理員！", "天堂");
			}
		}
		
        private void Form2_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false; // 跨线程访问窗口句柄
            Bitmap bulletin = Resources.Bulletin;
            if (Program.IsSkinEnable() && System.IO.File.Exists(Program.GetBulletin()))
            {
                bulletin = new Bitmap(Program.GetBulletin());
            }
            BitmapRegion region = new BitmapRegion();
            BitmapRegion.CreateControlRegion(this, bulletin);
            Win32API.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            LoadUpdate(); // 載入遠程更新列表信息
            // 根據遠程信息判定是否打開更新線程
            if (Ini.GetUpdateFiles().Count > 0)
            {
                this._backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                this._pictureBox1.Enabled = true; // 按鈕解鎖
            }
            
            this._labVerInfo.Text = "Press 'Start' to play Lineage.";
        }

        private void Form2_MouseClick(object sender, MouseEventArgs e)
        {
            if ((((e.Button == MouseButtons.Left) && (e.X > 0x29a)) && ((e.X < 0x2ac) && (e.Y > 6))) && (e.Y < 0x19))
            {
                base.DialogResult = DialogResult.Cancel;
                base.Close();
            }
            else if ((((e.Button == MouseButtons.Left) && (e.X > 0x237)) && ((e.X < 0x299) && (e.Y > 0x19d))) && (e.Y < 0x1c0))
            {
                base.DialogResult = DialogResult.Cancel;
                base.Close();
            }
            else if ((((e.Button == MouseButtons.Left) && (e.X > 0x237)) && ((e.X < 0x299) && (e.Y > 0x16f))) && (e.Y < 0x19c))
            {
            }
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this._isFirst)
                {
                    this._prevLeftClick = new Point(e.X, e.Y);
                    this._isFirst = false;
                }
                else
                {
                    if (!this._toBlock)
                    {
                        base.Location = new Point((base.Location.X + e.X) - this._prevLeftClick.X, (base.Location.Y + e.Y) - this._prevLeftClick.Y);
                    }
                    this._prevLeftClick = new Point(e.X, e.Y);
                    this._toBlock = !this._toBlock;
                }
            }
            else
            {
                this._isFirst = true;
            }
            if ((((e.X > 0x29a) && (e.X < 0x2ac)) && (e.Y > 6)) && (e.Y < 0x19))
            {
                this.Cursor = Cursors.Hand;
            }
            else if ((((e.X > 0x237) && (e.X < 0x299)) && (e.Y > 0x19d)) && (e.Y < 0x1c0))
            {
                this.Cursor = Cursors.Hand;
            }
            else if ((((e.X > 0x237) && (e.X < 0x299)) && (e.Y > 0x16f)) && (e.Y < 0x19c))
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            this._pictureBox1.Visible = true;
            this._pictureBox2.Visible = true;
            this._pictureBox10.Visible = true;
            this._blueProcessBar.Visible = true;
            this._redProcessBar.Visible = true;
        }

        private void InitializeComponent()
        {
            this._webBrowser1 = new System.Windows.Forms.WebBrowser();
            this._labVerInfo = new System.Windows.Forms.Label();
            this._pictureBox1 = new System.Windows.Forms.PictureBox();
            this._pictureBox2 = new System.Windows.Forms.PictureBox();
            this._pictureBox10 = new System.Windows.Forms.PictureBox();
            this._blueProcessBar = new System.Windows.Forms.PictureBox();
            this._redProcessBar = new System.Windows.Forms.PictureBox();
            this._backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._blueProcessBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._redProcessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this._webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this._webBrowser1.Location = new System.Drawing.Point(20, 123);
            this._webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this._webBrowser1.Name = "webBrowser1";
            this._webBrowser1.Size = new System.Drawing.Size(658, 228);
            this._webBrowser1.TabIndex = 0;
            this._webBrowser1.TabStop = false;
            this._webBrowser1.WebBrowserShortcutsEnabled = false;
            this._webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // labVerInfo
            // 
            this._labVerInfo.AutoSize = true;
            this._labVerInfo.BackColor = System.Drawing.Color.Transparent;
            this._labVerInfo.Font = new System.Drawing.Font("細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._labVerInfo.ForeColor = System.Drawing.Color.Bisque;
            this._labVerInfo.Location = new System.Drawing.Point(182, 383);
            this._labVerInfo.Name = "labVerInfo";
            this._labVerInfo.Size = new System.Drawing.Size(179, 12);
            this._labVerInfo.TabIndex = 9;
            this._labVerInfo.Text = "Checking updated resources...";
            // 
            // pictureBox1
            // 
            this._pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this._pictureBox1.Image = global::SFL.Properties.Resources.ImgBtnStart;
            this._pictureBox1.Location = new System.Drawing.Point(574, 374);
            this._pictureBox1.Name = "pictureBox1";
            this._pictureBox1.Size = new System.Drawing.Size(94, 44);
            this._pictureBox1.TabIndex = 10;
            this._pictureBox1.TabStop = false;
            this._pictureBox1.Visible = false;
            this._pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this._pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this._pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // pictureBox2
            // 
            this._pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this._pictureBox2.Image = global::SFL.Properties.Resources.ImgBtnCancel2;
            this._pictureBox2.Location = new System.Drawing.Point(575, 420);
            this._pictureBox2.Name = "pictureBox2";
            this._pictureBox2.Size = new System.Drawing.Size(96, 32);
            this._pictureBox2.TabIndex = 11;
            this._pictureBox2.TabStop = false;
            this._pictureBox2.Visible = false;
            this._pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            this._pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
            // 
            // pictureBox10
            // 
            this._pictureBox10.BackColor = System.Drawing.Color.Transparent;
            this._pictureBox10.Image = global::SFL.Properties.Resources.ImgBtnCloseClick;
            this._pictureBox10.Location = new System.Drawing.Point(670, 12);
            this._pictureBox10.Name = "pictureBox10";
            this._pictureBox10.Size = new System.Drawing.Size(14, 14);
            this._pictureBox10.TabIndex = 21;
            this._pictureBox10.TabStop = false;
            this._pictureBox10.Visible = false;
            this._pictureBox10.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox10_MouseDown);
            this._pictureBox10.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox10_MouseUp);
            // 
            // BlueProcessBar
            // 
            this._blueProcessBar.BackColor = System.Drawing.Color.Transparent;
            this._blueProcessBar.Image = global::SFL.Properties.Resources.fr08;
            this._blueProcessBar.Location = new System.Drawing.Point(114, 409);
            this._blueProcessBar.Name = "BlueProcessBar";
            this._blueProcessBar.Size = new System.Drawing.Size(372, 15);
            this._blueProcessBar.TabIndex = 22;
            this._blueProcessBar.TabStop = false;
            this._blueProcessBar.Visible = false;
            // 
            // RedProcessBar
            // 
            this._redProcessBar.BackColor = System.Drawing.Color.Transparent;
            this._redProcessBar.Image = global::SFL.Properties.Resources.fr09;
            this._redProcessBar.Location = new System.Drawing.Point(114, 426);
            this._redProcessBar.Name = "RedProcessBar";
            this._redProcessBar.Size = new System.Drawing.Size(374, 13);
            this._redProcessBar.TabIndex = 23;
            this._redProcessBar.TabStop = false;
            this._redProcessBar.Visible = false;
            // 
            // backgroundWorker1
            // 
            this._backgroundWorker1.WorkerReportsProgress = true;
            this._backgroundWorker1.WorkerSupportsCancellation = true;
            this._backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this._backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this._backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // Form2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::SFL.Properties.Resources.Bulletin;
            this.ClientSize = new System.Drawing.Size(692, 461);
            this.ControlBox = false;
            this.Controls.Add(this._redProcessBar);
            this.Controls.Add(this._blueProcessBar);
            this.Controls.Add(this._pictureBox10);
            this.Controls.Add(this._pictureBox2);
            this.Controls.Add(this._pictureBox1);
            this.Controls.Add(this._labVerInfo);
            this.Controls.Add(this._webBrowser1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form2";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SFL";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseClick);
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._blueProcessBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._redProcessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this._pictureBox1.Image = Resources.ImgBtnStartClick;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            this._pictureBox1.Image = Resources.ImgBtnStart;
            if ((((e.X > 0) && (e.X < this._pictureBox1.Width)) && (e.Y > 0)) && (e.Y < this._pictureBox1.Height))
            {
                try
                {
                    if (!this._eatPro.HasExited)
                    {
                        return;
                    }
                }
                catch
                {
                }
                base.DialogResult = DialogResult.OK;
            }
        }

        private void pictureBox10_MouseDown(object sender, MouseEventArgs e)
        {
            this._pictureBox10.Image = Resources.ImgBtnClose;
        }

        private void pictureBox10_MouseUp(object sender, MouseEventArgs e)
        {
            this._pictureBox10.Image = Resources.ImgBtnCloseClick;
            if ((((e.X > 0) && (e.X < this._pictureBox10.Width)) && (e.Y > 0)) && (e.Y < this._pictureBox10.Height))
            {
                base.DialogResult = DialogResult.Cancel;
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            this._pictureBox2.Image = Resources.ImgBtnCancel2Click;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            this._pictureBox2.Image = Resources.ImgBtnCancel2;
            if ((((e.X > 0) && (e.X < this._pictureBox2.Width)) && (e.Y > 0)) && (e.Y < this._pictureBox2.Height))
            {
                this._backgroundWorker1.CancelAsync();
                base.DialogResult = DialogResult.Cancel;
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private void webc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BabyWebClient client = (BabyWebClient)sender;
            this._downloadFileslist.Add(client.TargetFile);
            int num = (int)((this._downloadFileslist.Count / this._countdownloadFiles) * 100M);
            this.RedProcessValue = num;
            UnZipFloClass class2 = new UnZipFloClass();
            try
            {
            	string strIni;
            	class2.UnZipFile(Application.StartupPath + "//" + client.TargetFile, Application.StartupPath, out strIni);
                System.IO.File.Delete(Application.StartupPath + "//" + client.TargetFile);
            	
                string strUpdate;
                class2.UnZipFile(Application.StartupPath + "//UpdateTemp//" + client.TargetFile, Application.StartupPath, out strUpdate);
                System.IO.File.Delete(Application.StartupPath + "//UpdateTemp//" + client.TargetFile);
            }
            catch
            {
                Console.WriteLine("文件解压出错了啊！");
            }
        }

        private void webc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            BabyWebClient client = (BabyWebClient)sender;
            Console.WriteLine("{3} {0}/{1} {2}%", new object[] { e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage, client.TargetFile });
            double rate = 0.00D;
            rate = ((double) this._downloadFileslist.Count) / ((double) this._countdownloadFiles);
            int num = (int)(rate * 100);
            this._labVerInfo.Text = string.Format("Receiving file...{0} {2} bytes. {3}/{4}", new object[] { client.CurrentFile, e.BytesReceived, e.TotalBytesToReceive, this._downloadFileslist.Count + 1, this._countdownloadFiles });
            this.BlueProcessValue = e.ProgressPercentage;
            this.RedProcessValue = num;
        }

        public int BlueProcessValue
        {
            get
            {
                return this._blueProcessValue;
            }
            set
            {
                if (value < 0)
                {
                    this._blueProcessValue = 0;
                }
                else if (value > 100)
                {
                    this._blueProcessValue = 100;
                }
                else
                {
                    this._blueProcessValue = value;
                }
                this._blueProcessBar.Width = (int)(3.75 * this._blueProcessValue);
            }
        }

        public int RedProcessValue
        {
            get
            {
                return this._redProcessValue;
            }
            set
            {
                if (value < 0)
                {
                    this._redProcessValue = 0;
                }
                else if (value > 100)
                {
                    this._redProcessValue = 100;
                }
                else
                {
                    this._redProcessValue = value;
                }
                this._redProcessBar.Width = (int)(3.75 * this._redProcessValue);
            }
        }
    }
}


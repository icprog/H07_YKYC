using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Net.Sockets;

namespace H07_YKYC
{
    public partial class MainForm : Form
    {
        public string path = Program.GetStartupPath() + @"SaveData\";

        public string Path = null;          //程序运行的目录
        public DateTime startDT;
        public SettingForm mySettingForm;
        public QueryForm myQueryForm;
        public SaveFile mySaveFileThread;


        ServerAPP myServer = new ServerAPP();
        public int TagLock;

        public Queue<string[]> YKQueue = new Queue<string[]>();  //用于转存遥控日志
        public Queue<string[]> YKLogQueue = new Queue<string[]>();  //用于遥控日志显示存储

        //初始化String数组，用于遥控日志存储显示
        string[] YkContent = new string[10] { "time", "mode", "代号", "名称", "发送通道", "明", "密钥1", "算法参数", "比对结果", "代号源码" };

        ManualResetEvent WaitXHLResult = new ManualResetEvent(false);

        public bool ServerLedThreadTag = false;
        public bool ServerLedThreadTag2 = false;

        //创建K令码表源文件数组
        //public byte[] KcmdText;

        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        public bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public MainForm()
        {
            InitializeComponent();
            mySettingForm = new SettingForm(this);
            myQueryForm = new QueryForm(this);

            Path = Program.GetStartupPath();

            //启动日志
            MyLog.richTextBox1 = richTextBox1;
            MyLog.path = Program.GetStartupPath() + @"LogData\";
            MyLog.lines = 50;
            MyLog.start();
            startDT = System.DateTime.Now;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Data.DealCRTa.CRTName = "USB应答机A";
                Data.DealCRTa.XHLEnable = true;
                Data.DealCRTa.Led = pictureBox_CRTa;
                Data.DealCRTa.init();


                toolStripStatusLabel2.Text = "存储路径" + Path;

                mySaveFileThread = new SaveFile();
                mySaveFileThread.FileInit();
                mySaveFileThread.FileSaveStart();

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            try
            {
                Data.dtVCDU.Columns.Add("版本号", typeof(string));
                Data.dtVCDU.Columns.Add("SCID", typeof(string));
                Data.dtVCDU.Columns.Add("VCID", typeof(string));
                Data.dtVCDU.Columns.Add("虚拟信道帧计数", typeof(string));
                Data.dtVCDU.Columns.Add("回放", typeof(string));
                Data.dtVCDU.Columns.Add("保留", typeof(string));
                Data.dtVCDU.Columns.Add("插入域", typeof(string));

                DataRow dr = Data.dtVCDU.NewRow();
                dr["版本号"] = "01";
                dr["SCID"] = "07";
                dr["VCID"] = "06";
                dr["虚拟信道帧计数"] = "000000";
                dr["回放"] = "00";
                dr["保留"] = "00";
                dr["插入域"] = "000000000000";
                Data.dtVCDU.Rows.Add(dr);

                dataGridView_VCDU.DataSource = Data.dtVCDU;
                dataGridView_VCDU.AllowUserToAddRows = false;


                Data.dtYK.Columns.Add("名称", typeof(string));
                Data.dtYK.Columns.Add("数量", typeof(Int32));

                DataRow dr1 = Data.dtYK.NewRow();
                dr1["名称"] = "遥控数据帧接收总数量";
                dr1["数量"] = 0;
                Data.dtYK.Rows.Add(dr1);

                dr1 = Data.dtYK.NewRow();
                dr1["名称"] = "信息格式正确帧数量";
                dr1["数量"] = 0;
                Data.dtYK.Rows.Add(dr1);

                dr1 = Data.dtYK.NewRow();
                dr1["名称"] = "信息格式错误帧数量";
                dr1["数量"] = 0;
                Data.dtYK.Rows.Add(dr1);

                dr1 = Data.dtYK.NewRow();
                dr1["名称"] = "成功转发帧数量";
                dr1["数量"] = 0;
                Data.dtYK.Rows.Add(dr1);

                dr1 = Data.dtYK.NewRow();
                dr1["名称"] = "失败转发帧数量";
                dr1["数量"] = 0;
                Data.dtYK.Rows.Add(dr1);

                dataGridView1.DataSource = Data.dtYK;
                dataGridView1.AllowUserToAddRows = false;

                Data.dtYC.Columns.Add("名称", typeof(string));
                Data.dtYC.Columns.Add("数量", typeof(Int32));

                DataRow dr2 = Data.dtYC.NewRow();
                dr2["名称"] = "遥测数据帧接收总数量";
                dr2["数量"] = 0;
                Data.dtYC.Rows.Add(dr2);

                dr2 = Data.dtYC.NewRow();
                dr2["名称"] = "校验正确帧数量";
                dr2["数量"] = 0;
                Data.dtYC.Rows.Add(dr2);

                dr2 = Data.dtYC.NewRow();
                dr2["名称"] = "校验错误帧数量";
                dr2["数量"] = 0;
                Data.dtYC.Rows.Add(dr2);

                dr2 = Data.dtYC.NewRow();
                dr2["名称"] = "成功转发帧数量";
                dr2["数量"] = 0;
                Data.dtYC.Rows.Add(dr2);

                dr2 = Data.dtYC.NewRow();
                dr2["名称"] = "失败转发帧数量";
                dr2["数量"] = 0;
                Data.dtYC.Rows.Add(dr2);

                dataGridView2.DataSource = Data.dtYC;
                dataGridView2.AllowUserToAddRows = false;


            }
            catch (Exception ex)
            {
                Trace.WriteLine("初始化DataTable Failed：" + ex.Message);
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btn_ZK1_Close.Enabled) btn_ZK1_Close_Click(sender, e);
                if (btn_ZK1_YC_Close.Enabled) btn_ZK1_YC_Close_Click(sender, e);

                if(btn_CRTa_Open.Enabled ==false)
                {
                    ClientAPP.Disconnect(ref ClientAPP.Server_CRTa);
                    ClientAPP.Disconnect(ref ClientAPP.Server_CRTa_Return);
                    btn_CRTa_Open.Enabled = true;
                    btn_CRTa_Close.Enabled = false;
                    Data.DealCRTa.LedOff();
                    MyLog.Info("关闭连接--USB应答机A");
                }

                Thread.Sleep(100);
                mySaveFileThread.FileClose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }


        #region MainForm_Paint
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Trace.WriteLine("MainForm_Paint");

        }
        #endregion

        public bool Logform_state = true;
        public int LogWaitTime = 600;
        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = "剩余空间" + DiskInfo.GetFreeSpace(Path[0].ToString()) + "MB";
            toolStripStatusLabel5.Text = "当前时间：" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " ";

            TimeSpan ts = DateTime.Now.Subtract(startDT);
            toolStripStatusLabel6.Text = "已运行：" + ts.Days.ToString() + "天" +
                ts.Hours.ToString() + "时" +
                ts.Minutes.ToString() + "分" +
                ts.Seconds.ToString() + "秒";

        }

        private void 系统设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyLog.Info("进行系统设置");
            if (mySettingForm != null)
            {
                mySettingForm.Activate();
            }
            else
            {
                mySettingForm = new SettingForm(this);
            }
            mySettingForm.ShowDialog();
        }



        private void 运行日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Program.GetStartupPath() + @"LogData\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //   openFileDialog1.InitialDirectory = Program.GetStartupPath() + @"LogData\";
                Process Pnoted = new Process();
                try
                {
                    Pnoted.StartInfo.FileName = openFileDialog1.FileName;
                    Pnoted.Start();
                }
                catch
                {
                    //MessageBox.Show("运行日志打开失败！");
                }
            }
        }


        private void 遥控日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Program.GetStartupPath() + @"LogData\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //   openFileDialog1.InitialDirectory = Program.GetStartupPath() + @"LogData\";
                Process Pnoted = new Process();
                try
                {
                    Pnoted.StartInfo.FileName = openFileDialog1.FileName;
                    Pnoted.Start();
                }
                catch
                {
                    //MessageBox.Show("运行日志打开失败！");
                }
            }
        }

        private void 数据查询和回放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myQueryForm != null)
            {
                myQueryForm.Activate();
            }
            else
            {
                myQueryForm = new QueryForm(this);
            }
            myQueryForm.ShowDialog();
        }


        void DealCRT_On(ref Data.CRT_STRUCT myCRT)
        {
            myCRT.LedOn();
            MyLog.Info("连接成功--" + myCRT.CRTName);

        }
        void DealCRT_Off(ref Data.CRT_STRUCT myCRT)
        {
            myCRT.LedOff();
            MyLog.Info("无法连接--" + myCRT.CRTName);
        }
        private void buttonCRT_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btn_CRTa_Open":
                    MyLog.Info("尝试连接--USB应答机A...");
                    ClientAPP.Server_CRTa.ServerIP = ConfigurationManager.AppSettings["Server_CRTa_Ip"];
                    ClientAPP.Server_CRTa.ServerPORT = ConfigurationManager.AppSettings["Server_CRTa_Port"];
                    ClientAPP.Connect(ref ClientAPP.Server_CRTa);
                    if (ClientAPP.Server_CRTa.IsConnected)
                    {
                        DealCRT_On(ref Data.DealCRTa);
                        MyLog.Info("连接成功--" + Data.DealCRTa.CRTName + "--遥控端口");
                        new Thread(() => { Fun_Transfer2CRT(ref Data.DealCRTa, ref ClientAPP.Server_CRTa, ref SaveFile.DataQueue_out2); }).Start();
                        new Thread(() => { Fun_RecvFromCRT(ref Data.DealCRTa, ref ClientAPP.Server_CRTa); }).Start();
                    }
                    else
                    {
                        DealCRT_Off(ref Data.DealCRTa);
                        return;
                    }
                    btn_CRTa_Open.Enabled = false;
                    btn_CRTa_Close.Enabled = true;

                    ClientAPP.Server_CRTa_Return.ServerIP = ConfigurationManager.AppSettings["Server_CRTa_Ip"];
                    ClientAPP.Server_CRTa_Return.ServerPORT = "3070";
                    ClientAPP.Connect(ref ClientAPP.Server_CRTa_Return);
                    if (ClientAPP.Server_CRTa_Return.IsConnected)
                    {
                        MyLog.Info("连接成功--" + Data.DealCRTa.CRTName + "--遥测端口");
                        new Thread(() => { Fun_RecvFromCRT_Return(ref Data.DealCRTa, ref ClientAPP.Server_CRTa_Return); }).Start();
                    }

                    break;
                case "btn_CRTa_Close":
                    ClientAPP.Disconnect(ref ClientAPP.Server_CRTa);
                    ClientAPP.Disconnect(ref ClientAPP.Server_CRTa_Return);
                    btn_CRTa_Open.Enabled = true;
                    btn_CRTa_Close.Enabled = false;
                    Data.DealCRTa.LedOff();
                    MyLog.Info("关闭连接--USB应答机A");
                    break;

                default:
                    break;
            }
        }

        public delegate void UpdateText(string str, TextBox mytextbox);
        public void UpdateTextMethod(string str, TextBox mytextbox)
        {
            mytextbox.Text = str;
        }


        private void Fun_RecvFromCRT(ref Data.CRT_STRUCT myCRT, ref ClientAPP.TCP_STRUCT Server_CRT)
        {
            Trace.WriteLine("Entering" + myCRT.CRTName + "Fun_RecvFromCRT!!");

            Delegate la = new UpdateText(UpdateTextMethod);

            while (Server_CRT.IsConnected)
            {
                try
                {
                    byte[] RecvBufCRTa = new byte[1024];
                    int RecvNum = Server_CRT.sck.Receive(RecvBufCRTa);
                    if (RecvNum > 0)
                    {
                        int[] RecvBufInt = Program.BytesToInt(RecvBufCRTa);

                        myCRT.TCMsgStatus = RecvBufInt[7];

                        this.Invoke(la, RecvBufInt[7].ToString(), myCRT.mytextbox);

                    }
                    else
                    {
                        Trace.WriteLine("收到数据少于等于0！");
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Trace.WriteLine("Exception leave!!");
                    break;
                }
            }
        }


        private void Fun_RecvFromCRT_Return(ref Data.CRT_STRUCT myCRT, ref ClientAPP.TCP_STRUCT Server_CRT)
        {
            Trace.WriteLine("++++++++++Entering" + myCRT.CRTName + "Fun_RecvFromCRT_Return!!");

            //int[] SendReq;
            //byte[] SendReqBytes;
            ////send request
            //SendReq = new int[16];
            //SendReq[0] = 1234567890;        //Start of msg
            //SendReq[1] = 64;                //Size of msg in bytes
            //SendReq[2] = 0;                 //Size of msg
            //SendReq[3] = 0;                 //0:channelA 1:channelB
            //SendReq[4] = 0;                 //Real time telemetry
            //SendReq[5] = 0;                 //Permanent flow 一次请求
            //SendReq[15] = -1234567890;
            //SendReqBytes = Program.IntToBytes(SendReq);

            //int len = Server_CRT.sck.Send(SendReqBytes);

            while (Server_CRT.IsConnected)
            {
                try
                {
                    byte[] RecvBufCRTa = new byte[200];
                    int RecvNum = Server_CRT.sck.Receive(RecvBufCRTa);

                    if (RecvNum > 0)
                    {
                        //TempRecvBuf 本次收到的数据
                        byte[] TempRecvBuf = new byte[RecvNum];
                        Array.Copy(RecvBufCRTa, TempRecvBuf, RecvNum);

                        SaveFile.Lock_Dat3.EnterWriteLock();
                        SaveFile.DataQueue_out3.Enqueue(TempRecvBuf);
                        SaveFile.Lock_Dat3.ExitWriteLock();

                        String tempstr = "";
                        for (int i = 0; i < TempRecvBuf.Length; i++)
                        {
                            tempstr += TempRecvBuf[i].ToString("x2");
                        }
                        Trace.WriteLine(tempstr);

                        Data.dtYC.Rows[0]["数量"] = (int)Data.dtYC.Rows[0]["数量"] + 1; //收到总数

                        //YCBuf 本次收到的实际遥测数据
                        byte[] YCBuf = new byte[RecvNum - 68];
                        Array.Copy(RecvBufCRTa, 64, YCBuf, 0, RecvNum - 68);

                        SaveFile.Lock_Dat4.EnterWriteLock();
                        SaveFile.DataQueue_out4.Enqueue(YCBuf);
                        SaveFile.Lock_Dat4.ExitWriteLock();

                        String tempstr2 = "";
                        for (int i = 0; i < YCBuf.Length; i++)
                        {
                            tempstr2 += YCBuf[i].ToString("x2");
                        }
                        Trace.WriteLine(tempstr2);

                        //ushort CRC = 0xffff;
                        //ushort genpoly = 0x1021;
                        //for (int i = 0; i < YCBuf.Length-2; i = i + 1)
                        //{
                        //    CRC = Function.CRChware(YCBuf[i], genpoly, CRC);
                        //}
                        ////      MyLog.Info("Calc 通道1 CRC = " + CRC.ToString("x4"));
                        //Trace.WriteLine("Calc 通道1 CRC = " + CRC.ToString("x4"));
                    }
                    else
                    {
                        Trace.WriteLine("收到数据少于等于0！");
                        break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    break;
                }
            }
            Trace.WriteLine("----------Leaving" + myCRT.CRTName + "Fun_RecvFromCRT_Return!!");
        }

        private void Fun_Transfer2CRT(ref Data.CRT_STRUCT myCRT, ref ClientAPP.TCP_STRUCT Server_CRT, ref Queue<byte[]> DataQueue_save)
        {
            Delegate la = new UpdateText(UpdateTextMethod);

            while (Server_CRT.IsConnected)
            {
                if (myCRT.DataQueue_CRT.Count() > 0)
                {
                    byte[] SendByte = myCRT.DataQueue_CRT.Dequeue();
                    Server_CRT.sck.Send(SendByte);

                    Data.dtYK.Rows[3]["数量"] = (int)Data.dtYK.Rows[3]["数量"] + 1;


                    //增加存储，DataQueue_save为引用的对应SaveFile里面的Queue
                    DataQueue_save.Enqueue(SendByte);

                    myCRT.Transfer2CRTa_TempStr = "";
                    for (int m = 24; m < SendByte.Length - 8; m++)
                    {
                        myCRT.Transfer2CRTa_TempStr += SendByte[m].ToString("x2");
                    }
                    Trace.WriteLine("Fun_Transfer2CRT:" + myCRT.Transfer2CRTa_TempStr);

                }

            }
        }

        /// <summary>
        /// 十六进制String转化为BYTE数组
        /// </summary>
        /// <param name="hexString">参数：输入的十六进制String</param>
        /// <returns>BYTE数组</returns>
        private static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];

            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;

        }

        /// <summary>
        /// 启动服务器Socket监听总控设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ZK1_Open_Click(object sender, EventArgs e)
        {
            ServerLedThreadTag = true;
            new Thread(() => { ServerConnect(); }).Start();
            myServer.ServerStart();

            if (myServer.ServerOn)
            {
                btn_ZK1_Open.Enabled = false;
                btn_ZK1_Close.Enabled = true;
            }
            else
            {
                btn_ZK1_Close.Enabled = false;
                btn_ZK1_Open.Enabled = true;
            }
        }

        /// <summary>
        /// 关闭服务器Socket，断开与总控设备连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ZK1_Close_Click(object sender, EventArgs e)
        {
            ServerLedThreadTag = false;
            myServer.ServerStop();

            btn_ZK1_Close.Enabled = false;
            btn_ZK1_Open.Enabled = true;
        }




        private void 启动toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (启动toolStripMenuItem.Text == "启动")
            {
                btn_ZK1_Open_Click(sender, e);
                btn_ZK1_YC_Open_Click(sender, e);
                //     buttonCRT_Click(btn_CRTa_Open, e);
                //     buttonCRT_Click(btn_CRTb_Open, e);

                启动toolStripMenuItem.Text = "停止";
            }
            else
            {
                btn_ZK1_Close_Click(sender, e);
                btn_ZK1_YC_Close_Click(sender, e);
                //   buttonCRT_Click(btn_CRTa_Close, e);
                //   buttonCRT_Click(btn_CRTb_Close, e);


                启动toolStripMenuItem.Text = "启动";
            }
        }

        private void btn_ZK1_YC_Open_Click(object sender, EventArgs e)
        {
            ServerLedThreadTag2 = true;
            new Thread(() => { ServerConnect2(); }).Start();
            myServer.ServerStart2();

            if (myServer.ServerOn_YC)
            {
                btn_ZK1_YC_Open.Enabled = false;
                btn_ZK1_YC_Close.Enabled = true;
            }
            else
            {
                btn_ZK1_YC_Close.Enabled = false;
                btn_ZK1_YC_Open.Enabled = true;
            }
        }

        private void btn_ZK1_YC_Close_Click(object sender, EventArgs e)
        {
            ServerLedThreadTag2 = false;
            myServer.ServerStop2();

            btn_ZK1_YC_Close.Enabled = false;
            btn_ZK1_YC_Open.Enabled = true;
        }

        private void ServerConnect()
        {
            Trace.WriteLine("Enter-------ServerConnect1");
            while (ServerLedThreadTag)
            {
                Data.ServerConnectEvent.WaitOne();
                Data.ServerConnectEvent.Reset();

                if (ClientAPP.ClientZK1.IsConnected)
                    pictureBox_ZK1.Image = Properties.Resources.green2;
                else
                    pictureBox_ZK1.Image = Properties.Resources.red2;

            }
            Trace.WriteLine("Leave------ServerConnect1");
        }

        private void ServerConnect2()
        {
            Trace.WriteLine("Enter-------ServerConnect2");
            while (ServerLedThreadTag2)
            {
                Data.ServerConnectEvent2.WaitOne();
                Data.ServerConnectEvent2.Reset();

                if (ClientAPP.ClientZK1_YC.IsConnected)
                    pictureBox_ZK1_YC.Image = Properties.Resources.green2;
                else
                    pictureBox_ZK1_YC.Image = Properties.Resources.red2;

            }
            Trace.WriteLine("Leave------ServerConnect2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] VCDU = new byte[1018];

            byte b1 = byte.Parse((string)Data.dtVCDU.Rows[0]["版本号"], System.Globalization.NumberStyles.HexNumber);
            byte b2 = byte.Parse((string)Data.dtVCDU.Rows[0]["SCID"], System.Globalization.NumberStyles.HexNumber);
            byte b3 = byte.Parse((string)Data.dtVCDU.Rows[0]["VCID"], System.Globalization.NumberStyles.HexNumber);
            VCDU[0] = (byte)((byte)(b1 << 6) + (byte)(b2 >> 2));
            VCDU[1] = (byte)((byte)(b2 << 6) + b3);
            string temp = ((string)Data.dtVCDU.Rows[0]["虚拟信道帧计数"]).PadLeft(6, '0');
            VCDU[2] = byte.Parse(temp.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            VCDU[3] = byte.Parse(temp.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            VCDU[4] = byte.Parse(temp.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte b4 = byte.Parse((string)Data.dtVCDU.Rows[0]["回放"], System.Globalization.NumberStyles.HexNumber);
            byte b5 = byte.Parse((string)Data.dtVCDU.Rows[0]["保留"], System.Globalization.NumberStyles.HexNumber);
            VCDU[5] = (byte)((byte)(b4 << 7) + b5);

            byte[] time_login = new byte[6];
            time_login = Function.Get_Time();
            time_login.CopyTo(VCDU, 6);//时间

            Data.dtVCDU.Rows[0]["插入域"] = time_login[0].ToString("x2") + time_login[1].ToString("x2")
                + time_login[2].ToString("x2") + time_login[3].ToString("x2")
                + time_login[4].ToString("x2") + time_login[5].ToString("x2");

            for (int i = 12; i < 1018; i++) VCDU[i] = (byte)i;

            byte[] Return_Send = Function.Make_tozk_YC_frame(Data.Data_Flag_Real, Data.InfoFlag_DMTC, VCDU);
            if (myServer.ServerOn_YC)
            {
                Data.DataQueue_GT.Enqueue(Return_Send);
                MyLog.Info("手动发送一次遥测数据--成功");
            }
            else
            {
                Data.dtYC.Rows[4]["数量"] = (int)Data.dtYC.Rows[4]["数量"] + 1;
                MyLog.Error("手动发送一次遥测数据--失败");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in Data.dtYK.Rows)
            {
                dr["数量"] = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in Data.dtYC.Rows)
            {
                dr["数量"] = 0;
            }
        }

        private void btn_SendYC_Click(object sender, EventArgs e)
        {
            #region SJXL.Length = 255，用来解随机序列
            byte[] SJXL = new byte[] { 0xff, 0x39, 0x9e, 0x5a, 0x68, 0xe9, 0x06, 0xf5, 0x6c, 0x89, 0x2f, 0xa1, 0x31, 0x5e, 0x08, 0xc0, 0x52, 0xa8, 0xbb, 0xae, 0x4e, 0xc2, 0xc7, 0xed, 0x66, 0xdc, 0x38, 0xd4, 0xf8, 0x86, 0x50, 0x3d, 0xfe, 0x73, 0x3c, 0xb4, 0xd1, 0xd2, 0x0d, 0xea, 0xd9, 0x12, 0x5f, 0x42, 0x62, 0xbc, 0x11, 0x80, 0xa5, 0x51, 0x77, 0x5c, 0x9d, 0x85, 0x8f, 0xda, 0xcd, 0xb8, 0x71, 0xa9, 0xf1, 0x0c, 0xa0, 0x7b, 0xfc, 0xe6, 0x79, 0x69, 0xa3, 0xa4, 0x1b, 0xd5, 0xb2, 0x24, 0xbe, 0x84, 0xc5, 0x78, 0x23, 0x01, 0x4a, 0xa2, 0xee, 0xb9, 0x3b, 0x0b, 0x1f, 0xb5, 0x9b, 0x70, 0xe3, 0x53, 0xe2, 0x19, 0x40, 0xf7, 0xf9, 0xcc, 0xf2, 0xd3, 0x47, 0x48, 0x37, 0xab, 0x64, 0x49, 0x7d, 0x09, 0x8a, 0xf0, 0x46, 0x02, 0x95, 0x45, 0xdd, 0x72, 0x76, 0x16, 0x3f, 0x6b, 0x36, 0xe1, 0xc6, 0xa7, 0xc4, 0x32, 0x81, 0xef, 0xf3, 0x99, 0xe5, 0xa6, 0x8e, 0x90, 0x6f, 0x56, 0xc8, 0x92, 0xfa, 0x13, 0x15, 0xe0, 0x8c, 0x05, 0x2a, 0x8b, 0xba, 0xe4, 0xec, 0x2c, 0x7e, 0xd6, 0x6d, 0xc3, 0x8d, 0x4f, 0x88, 0x65, 0x03, 0xdf, 0xe7, 0x33, 0xcb, 0x4d, 0x1d, 0x20, 0xde, 0xad, 0x91, 0x25, 0xf4, 0x26, 0x2b, 0xc1, 0x18, 0x0a, 0x55, 0x17, 0x75, 0xc9, 0xd8, 0x58, 0xfd, 0xac, 0xdb, 0x87, 0x1a, 0x9f, 0x10, 0xca, 0x07, 0xbf, 0xce, 0x67, 0x96, 0x9a, 0x3a, 0x41, 0xbd, 0x5b, 0x22, 0x4b, 0xe8, 0x4c, 0x57, 0x82, 0x30, 0x14, 0xaa, 0x2e, 0xeb, 0x93, 0xb0, 0xb1, 0xfb, 0x59, 0xb7, 0x0e, 0x35, 0x3e, 0x21, 0x94, 0x0f, 0x7f, 0x9c, 0xcf, 0x2d, 0x34, 0x74, 0x83, 0x7a, 0xb6, 0x44, 0x97, 0xd0, 0x98, 0xaf, 0x04, 0x60, 0x29, 0x54, 0x5d, 0xd7, 0x27, 0x61, 0x63, 0xf6, 0xb3, 0x6e, 0x1c, 0x6a, 0x7c, 0x43, 0x28, 0x1e };
            #endregion

            string Str_Content = this.textBox1.Text.Replace(" ", "");
            int AddAlen = 16 - (Str_Content.Length % 16);
            Str_Content = Str_Content.PadRight(AddAlen, 'A');

            Trace.WriteLine("加扰前：" + Str_Content);

            byte[] temp = StrToHexByte(Str_Content);

            byte[] tempbuf = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                tempbuf[i] = (byte)(temp[i] ^ SJXL[i]);
            }

            string tempstr = "";
            for (int i = 0; i < tempbuf.Length; i++)
            {
                tempstr += tempbuf[i].ToString("x2");
            }

            Trace.WriteLine("加扰后：" + tempstr);

            tempstr = "eb90" + tempstr + "C5C5C5C5C5C579";
            Trace.WriteLine("加头尾：" + tempstr);

            byte[] FinalSend = StrToHexByte(tempstr);

            Data.DealCRTa.DataQueue_CRT.Enqueue(FinalSend);
        }
    }
}

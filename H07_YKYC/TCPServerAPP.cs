using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Win32;
using System.Diagnostics;
using System.Timers;

namespace H07_YKYC
{
    public class ServerAPP
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);


        public MainForm myForm;
        private static byte[] RecvBuf = new byte[1024];
        private static int ServerPort = 8888;
        static Socket ServerSocket;
        public bool ServerOn = false;//监听总控_YK
        List<Socket> ClientSocketList = new List<Socket>();

        private static int ServerPort2 = 7777;
        static Socket ServerSocket2;
        public bool ServerOn_YC = false;//监听总控_YC
        List<Socket> ClientSocketList2 = new List<Socket>();



        public void ServerStart()
        {
            Trace.WriteLine("------------------------进入ServerStart");
            ClientAPP.ClientZK1.ClientIP = ConfigurationManager.AppSettings["ZK1IP"];
            ServerOn = true;
            ClientAPP.ClientZK1.IsConnected = false;

            //     new Thread(() => { WhichZKOn(); }).Start();

            ServerPort = int.Parse(ConfigurationManager.AppSettings["LocalPort1_YK"]);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse(ConfigurationManager.AppSettings["LocalIP1"]);
            try
            {
                ServerSocket.Bind(new IPEndPoint(address, ServerPort));
                //  ServerSocket.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
                ServerSocket.Listen(10);
                ServerSocket.BeginAccept(new AsyncCallback(onCall), ServerSocket);//继续接受其他客户端的连接  
            }
            catch (Exception ex)
            {
                MyLog.Error("启动-->遥测-->服务器监听总控线程失败，检查IP设置和网络连接");
                Trace.WriteLine(ex.Message);
                ServerOn = false;
            }
            Trace.WriteLine("------------------------退出ServerStart");
        }

        public void ServerStart2()
        {
            Trace.WriteLine("------------------------进入ServerStart2");
            ClientAPP.ClientZK1_YC.ClientIP = ConfigurationManager.AppSettings["ZK1IP"];
            ServerOn_YC = true;
            ClientAPP.ClientZK1_YC.IsConnected = false;

            ServerPort2 = int.Parse(ConfigurationManager.AppSettings["LocalPort1_YC"]);
            ServerSocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address2 = IPAddress.Parse(ConfigurationManager.AppSettings["LocalIP1"]);
            try
            {
                ServerSocket2.Bind(new IPEndPoint(address2, ServerPort2));
                ServerSocket2.Listen(10);
                ServerSocket2.BeginAccept(new AsyncCallback(onCall_GT), ServerSocket2);
            }
            catch (Exception ex)
            {
                MyLog.Error("启动<--遥测<--服务器监听总控线程失败，检查IP设置和网络连接");
                Trace.WriteLine(ex.Message);
                ServerOn_YC = false;
            }
            Trace.WriteLine("------------------------退出ServerStart2");
        }

        public void ServerStop()
        {
            Trace.WriteLine("------------------------进入ServerStop");
            ServerOn = false;
            ClientAPP.ClientZK1.IsConnected = false;
            Data.ServerConnectEvent.Set();
            try
            {
                ServerSocket.Close();
                foreach (var sock in ClientSocketList)
                {
                    if (sock.Connected)
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        sock.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.Error("ServerStop无法关闭，错误原因:" + ex.Message);
            }
            Trace.WriteLine("------------------------退出ServerStop");
        }


        public void ServerStop2()
        {
            Trace.WriteLine("------------------------进入ServerStop2");
            ServerOn_YC = false;
            ClientAPP.ClientZK1_YC.IsConnected = false;
            Data.ServerConnectEvent2.Set();
            try
            {
                ServerSocket2.Close();
                foreach (var sock in ClientSocketList2)
                {
                    if (sock.Connected)
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        sock.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.Error("ServerStop2无法关闭，错误原因:" + ex.Message);
            }
            Trace.WriteLine("------------------------退出ServerStop2");
        }
        public void WhichZKOn()
        {
            Trace.WriteLine("------------------------进入WhichZKOn");
            while (ServerOn)
            {
                if (ClientAPP.ClientZK1.IsConnected)
                {
                    Data.MS1orMS2 = 1;
                    Data.WhoIsOnEvent.Set();
                }
                else if (ClientAPP.ClientZK2.IsConnected)
                {
                    Data.MS1orMS2 = 2;
                    Data.WhoIsOnEvent.Set();
                }
                else
                {
                    Data.MS1orMS2 = 0;
                    Data.WhoIsOnEvent.Set();
                }
            }

            Data.MS1orMS2 = 0;
            Data.WhoIsOnEvent.Set();
            Trace.WriteLine("------------------------退出WhichZKOn");
        }

        private void onCall(IAsyncResult ar)
        {
            Trace.WriteLine("onCall----遥控服务器");
            Socket serverSoc = (Socket)ar.AsyncState;
            if (ServerOn)
            {
                try
                {
                    Socket ClientSocket = serverSoc.EndAccept(ar);
                    if (serverSoc != null)
                    {
                        serverSoc.BeginAccept(new AsyncCallback(onCall), serverSoc);
                        IPEndPoint tmppoint = (IPEndPoint)ClientSocket.RemoteEndPoint;
                        String RemoteIpStr = tmppoint.Address.ToString();
                        Console.WriteLine(RemoteIpStr);
                        if (RemoteIpStr == ClientAPP.ClientZK1.ClientIP)
                        {
                            //----------发送登陆信息-----------
                            ClientSocket.Send(Function.Make_login_frame(Data.Data_Flag_Real, Data.ZK_S1));
                            MyLog.Info("遥控前端服务器---->向总控（主）发送登陆信息");

                            ClientAPP.ClientZK1.IsConnected = true;
                            Data.ServerConnectEvent.Set();

                            new Thread(() => { RecvFromClientZK1(ClientSocket); }).Start();
                            new Thread(() => { SendToClientZK1(ClientSocket); }).Start();

                            ClientSocketList.Add(ClientSocket);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Trace.WriteLine("onCall-Exception-遥控服务器" + ex.Message);  
                }
            }
            else
            {
                Console.WriteLine("Server already off!!!");
            }
        }


        private void onCall_GT(IAsyncResult ar)
        {
            Trace.WriteLine("onCall----遥测服务器");
            Socket serverSoc = (Socket)ar.AsyncState;
            if (ServerOn_YC)
            {
                try
                {
                    Socket ClientSocket = serverSoc.EndAccept(ar);
                    if (serverSoc != null)
                    {
                        serverSoc.BeginAccept(new AsyncCallback(onCall_GT), serverSoc);
                        IPEndPoint tmppoint = (IPEndPoint)ClientSocket.RemoteEndPoint;
                        String RemoteIpStr = tmppoint.Address.ToString();
                        Console.WriteLine(RemoteIpStr);
                        if (RemoteIpStr == ClientAPP.ClientZK1_YC.ClientIP)
                        {
                            //----------发送登陆信息-----------
                            ClientSocket.Send(Function.Make_login_frame(Data.Data_Flag_Real, Data.ZK_S1));

                            MyLog.Info("遥测前端服务器---->向总控（主）发送登陆信息");

                            ClientAPP.ClientZK1_YC.IsConnected = true;
                            Data.ServerConnectEvent2.Set();

                            new Thread(() => { RecvFromClientGT(ClientSocket); }).Start();
                            new Thread(() => { SendToClientGT(ClientSocket); }).Start();

                            ClientSocketList2.Add(ClientSocket);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Server already off!!!");
            }
        }

        /// <summary>
        /// 收到总控发来的网络数据包，解析并推入对应的队列中
        /// </summary>
        /// <param name="data"></param>
        public void deal_zk_data(byte[] data, int RecvNum)
        {
            byte[] DealData = new byte[RecvNum];//收到的实际数组，data后面可能包含0？
            Trace.WriteLine("收到数据量" + RecvNum.ToString());

            Array.Copy(data, DealData, RecvNum);

            string printstr = null;
            for (int i = 0; i < RecvNum; i++)
            {
                printstr += DealData[i].ToString("x2");
            }
            Trace.WriteLine("1111-------" + printstr);

            byte[] infoflag = new byte[4];
            infoflag[0] = byte.Parse(DealData[18].ToString());
            infoflag[1] = byte.Parse(DealData[19].ToString());
            infoflag[2] = byte.Parse(DealData[20].ToString());
            infoflag[3] = byte.Parse(DealData[21].ToString());
            string type_str = Encoding.ASCII.GetString(infoflag);
            Trace.WriteLine("type_str is :" + type_str);

            //   MyLog.Info("收到总控--：" + type_str + "数据量:" + RecvNum.ToString() + "内容：" + printstr);

            switch (type_str)
            {
                case "UCLK"://校时信息
                    //settime
                    //########
                    byte[] timedata = new byte[23];
                    Array.Copy(data, 29, timedata, 0, 23);
                    Function.Set_Time(timedata);
                    break;
                case "MASG"://主备当班
                            // Encoding.ASCII.GetString(RecvBufZK2, 0, RecvNum)
                            //解析判断
                    MyLog.Info(Encoding.ASCII.GetString(DealData, 29, RecvNum - 29));
                    Console.WriteLine("接受客户端{0}", Encoding.ASCII.GetString(DealData, 29, RecvNum - 29));
                    //返回应答信息码
                    //service.send(data)
                    break;
                case "CCUA"://总控-->遥控-->USB应答机a(遥控K令及密钥/算法注入)
                    DealWithZK2CRT_K(DealData, ref Data.DealCRTa);
                    break;

                case "DCUA"://总控-->遥控-->USB应答机a(遥控注数)
                    DealWithZK2CRT_PB(DealData, ref Data.DealCRTa);
                    break;

                case "DCUZ":
                case "CCUG":
                    //收到对地测控上行遥控数据
                    int len = DealData.Length - 45;

                    byte[] MsgHead = new byte[16];
                    byte[] MsgBody = new byte[len];
                    Array.Copy(DealData, 29, MsgHead, 0, 16);
                    Array.Copy(DealData, 45, MsgBody, 0, len);

                    //遥控注数应答
                    byte[] Return_data = new byte[len + 17];//16head+len(Body)，+1Byte retrun code
                    MsgHead.CopyTo(Return_data, 0);
                    MsgBody.CopyTo(Return_data, 4);
                    Return_data[len + 16] = 0x30;

                    byte[] CmdType = new byte[4];
                    CmdType[0] = byte.Parse(MsgBody[0].ToString());
                    CmdType[1] = byte.Parse(MsgBody[1].ToString());
                    CmdType[2] = byte.Parse(MsgBody[2].ToString());
                    CmdType[3] = byte.Parse(MsgBody[3].ToString());
                    string tempCmdTypeStr = Encoding.ASCII.GetString(CmdType);
                    if (tempCmdTypeStr != "0003")
                        Return_data[len + 16] = 0x31;
                    byte[] Return_Send = Function.Make_tozk_frame(Data.Data_Flag_Real, Data.InfoFlag_KACK, Return_data);
                    Data.DataQueue_ZK_ACK.Enqueue(Return_Send);

                    break;
                default:

                    break;
            }
        }

        private void DealWithZK2CRT_K(byte[] data, ref Data.CRT_STRUCT myCRT)
        {
            //Data.DealKSSA.DataQueue_CRT.Enqueue(FinalSendToCRT);
            byte[] MsgHead = new byte[4];
            byte[] MsgBody = new byte[12];
            Array.Copy(data, 29, MsgHead, 0, 4);
            Array.Copy(data, 33, MsgBody, 0, 12);

            byte[] KCmdByte = new byte[9];
            Function.Get_KcmdText(MsgBody[11], ref KCmdByte);

            byte[] ToBeSend = new byte[71];
            //将K令数据组成遥控帧序列后发送
            ToBeSend = Function.Make_toCRT_frame(KCmdByte, true);              //明指令
                                                                               //YKZL = Function.Make_toCRT_frame(temp2, false);                  //密指令
            byte[] FinalSendToCRT = Function.Make_toCortex_frame(ToBeSend);
            myCRT.DataQueue_CRT.Enqueue(FinalSendToCRT);

            //遥控指令应答
            byte[] Return_data = new byte[17];
            MsgHead.CopyTo(Return_data, 0);
            MsgBody.CopyTo(Return_data, 4);
            Return_data[16] = 0x30;
            byte[] Return_Send = Function.Make_tozk_frame(Data.Data_Flag_Real, Data.InfoFlag_CACK, Return_data);
            Data.DataQueue_ZK_ACK.Enqueue(Return_Send);

            byte[] Return_XHL = new byte[Return_Send.Length];
            Array.Copy(Return_Send, Return_XHL, Return_Send.Length);
            if (Data.WaitXHL_Return2ZK.WaitOne(500))
            {
                Data.WaitXHL_Return2ZK.Reset();
                Return_XHL[16] = Data.ReturnCode;
            }
            else
            {
                Return_XHL[16] = 0x37;//超时
            }
            Data.DataQueue_ZK_ACK.Enqueue(Return_XHL);

        }

        private void DealWithZK2CRT_PB(byte[] data, ref Data.CRT_STRUCT myCRT)
        {
            //To do here
            byte[] MsgHead = new byte[17];
            byte[] MsgBody = new byte[516];
            Array.Copy(data, 29, MsgHead, 0, 17);
            Array.Copy(data, 46, MsgBody, 0, 516);
            //并未对于明密态做出处理
            //Attention
            if (Data.MingMiTag)
            {
                MsgHead[4] = 0xF0;
            }
            else
            {
                MsgHead[4] = 0x0F;
                ushort CRC = 0xffff;
                ushort genpoly = 0x1021;
                for (int i = 2; i < 512; i = i + 1)
                {
                    CRC = Function.CRChware(MsgBody[i], genpoly, CRC);
                }
                MsgBody[512] = (byte)((CRC & 0xFF00) >> 2);
                MsgBody[513] = (byte)(CRC & 0x00FF);
            }

            byte[] FinalSendToCRT = Function.Make_toCortex_frame(MsgBody);
            myCRT.DataQueue_CRT.Enqueue(FinalSendToCRT);

            //遥控注数应答
            byte[] Return_data = new byte[534];//原数据包为533(4+12+1+516信息体)，+1Byte retrun code
            MsgHead.CopyTo(Return_data, 0);
            MsgBody.CopyTo(Return_data, 4);
            Return_data[533] = 0x30;
            byte[] Return_Send = Function.Make_tozk_frame(Data.Data_Flag_Real, Data.InfoFlag_KACK, Return_data);
            Data.DataQueue_ZK_ACK.Enqueue(Return_Send);

            byte[] Return_XHL = new byte[Return_Send.Length];
            Array.Copy(Return_Send, Return_XHL, Return_Send.Length);
            if (Data.WaitXHL_Return2ZK.WaitOne(500))
            {
                Data.WaitXHL_Return2ZK.Reset();
                Return_XHL[Return_XHL.Length - 1] = Data.ReturnCode;
            }
            else
            {
                Return_XHL[Return_XHL.Length - 1] = 0x37;//超时
            }
            Data.DataQueue_ZK_ACK.Enqueue(Return_XHL);
        }


        private void DealWithZK2CRT_YK(byte[] DealData)
        {
            //To do here
            byte[] MsgHead = new byte[16];
            int len = DealData.Length - 45;
            byte[] MsgBody = new byte[len];
            Array.Copy(DealData, 29, MsgHead, 0, 16);
            Array.Copy(DealData, 45, MsgBody, 0, len);

            //遥控注数应答
            byte[] Return_data = new byte[len + 17];//16head+len(Body)，+1Byte retrun code
            MsgHead.CopyTo(Return_data, 0);
            MsgBody.CopyTo(Return_data, 4);
            Return_data[len + 16] = 0x30;

            byte[] Return_Send = Function.Make_tozk_frame(Data.Data_Flag_Real, Data.InfoFlag_KACK, Return_data);
            Data.DataQueue_ZK_ACK.Enqueue(Return_Send);

        }

        private void SendToClientGT(object ClientSocket)
        {
            Trace.WriteLine("启动遥测前段设-->总控转发线程");
            Socket myClientSocket = (Socket)ClientSocket;
            while (ServerOn_YC && myClientSocket.Connected)
            {
                if (Data.DataQueue_GT.Count > 0)
                {
                    Byte[] temp = Data.DataQueue_GT.Dequeue();
                    myClientSocket.Send(temp);

                    SaveFile.DataQueue_out5.Enqueue(temp);

                    Data.dtYC.Rows[3]["数量"] = (int)Data.dtYC.Rows[3]["数量"] + 1;
                }
            }
        }

        private void SendToClientZK1(object ClientSocket)
        {
            Trace.WriteLine("启动遥测前段设备--》总控1发送线程");
            Socket myClientSocket = (Socket)ClientSocket;
            while (ServerOn && myClientSocket.Connected)
            {
                if (Data.DataQueue_ZK_ACK.Count > 0)
                {
                    myClientSocket.Send(Data.DataQueue_ZK_ACK.Dequeue());
                }
            }
        }

        private void RecvFromClientGT(object ClientSocket)
        {
            Trace.WriteLine("RecvFromClientZK1_YC!!");
            Socket myClientSocket = (Socket)ClientSocket;
            while (ServerOn_YC && myClientSocket.Connected)
            {
                try
                {
                    byte[] RecvBufZK1 = new byte[2048];
                    int RecvNum = myClientSocket.Receive(RecvBufZK1);
                    if (RecvNum > 0)
                    {
                        String tempstr = "";
                        byte[] RecvBufToFile = new byte[RecvNum];
                        for (int i = 0; i < RecvNum; i++)
                        {
                            RecvBufToFile[i] = RecvBufZK1[i];
                            tempstr += RecvBufZK1[i].ToString("x2");
                        }
                        Trace.WriteLine(tempstr);

                        //存储总控发来的
                        //  SaveFile.DataQueue_1.Enqueue(RecvBufZK1);

                        if (RecvNum > 29)
                        {
                            MyLog.Info("收到遥测数据量：" + RecvNum.ToString());
                            //Trace.WriteLine("网络收到数据量" + RecvNum.ToString());
                            deal_zk_data(RecvBufZK1, RecvNum);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("收到数据少于等于0！");
                        break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("RecvFromClientZK_YC Exception:" + e.Message);

                    if (myClientSocket.Connected)
                    {
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                    }
                    ClientAPP.ClientZK1_YC.IsConnected = false;

                    break;
                }
            }

            if (myClientSocket.Connected)
            {
                Trace.WriteLine("服务器主动关闭socket!");
                myClientSocket.Shutdown(SocketShutdown.Both);
                myClientSocket.Close();
            }

            ClientAPP.ClientZK1_YC.IsConnected = false;
            Trace.WriteLine("leave Server YC!!");
            Data.ServerConnectEvent2.Set();
        }

        private void RecvFromClientZK1(object ClientSocket)
        {
            Trace.WriteLine("RecvFromClientZK1!!");
            Socket myClientSocket = (Socket)ClientSocket;
            while (ServerOn && myClientSocket.Connected)
            {
                try
                {
                    byte[] RecvBufZK1 = new byte[4096];

                    int RecvNum = myClientSocket.Receive(RecvBufZK1);

                    if (RecvNum > 0)
                    {
                        Data.dtYK.Rows[0]["数量"] = (int)Data.dtYK.Rows[0]["数量"] + 1;

                        String tempstr = "";
                        byte[] RecvBufToFile = new byte[RecvNum];
                        for (int i = 0; i < RecvNum; i++)
                        {
                            RecvBufToFile[i] = RecvBufZK1[i];
                            tempstr += RecvBufZK1[i].ToString("x2");
                        }
                        Trace.WriteLine(tempstr);

                //        SaveFile.DataQueue_in1.Enqueue(tempstr);

                        SaveFile.DataQueue_out1.Enqueue(RecvBufToFile);

                        if (RecvNum > 45 && RecvNum < 300)
                        {
                            MyLog.Info("收到遥测数据量：" + RecvNum.ToString());
                            Data.dtYK.Rows[1]["数量"] = (int)Data.dtYK.Rows[1]["数量"] + 1;
                            deal_zk_data(RecvBufZK1, RecvNum);
                        }
                        else
                        {
                            Data.dtYK.Rows[2]["数量"] = (int)Data.dtYK.Rows[2]["数量"] + 1;
                        }
                    }
                    else
                    {
                        Trace.WriteLine("收到数据少于等于0！");
                        break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("RecvFromClientZK1 Exception:" + e.Message);
                    if (myClientSocket.Connected)
                    {
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                    }
                    ClientAPP.ClientZK1.IsConnected = false;
                    break;
                }
            }

            if (myClientSocket.Connected)
            {
                Trace.WriteLine("服务器主动关闭socket!");
                myClientSocket.Shutdown(SocketShutdown.Both);
                myClientSocket.Close();
            }

            ClientAPP.ClientZK1.IsConnected = false;
            Trace.WriteLine("leave RecvFromClientZK1!!");
            Data.ServerConnectEvent.Set();
        }   

    }
}

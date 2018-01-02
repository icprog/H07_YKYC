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
    class Data
    {


        public static DataTable dtVCDU = new DataTable();
        public static DataTable dtYK = new DataTable();//总控-->遥控服务器
        public static DataTable dtYC = new DataTable();//CRT-->遥测服务器

        //创建K令码表源文件数组
        public static byte[] KcmdText;

        //创建总控主备谁在当班的全局变量
        public static int MS1orMS2 = 1;
        public static ManualResetEvent WhoIsOnEvent = new ManualResetEvent(false);
        //创建本地控制远端控制的全局变量
        public static bool AutoCtrl = true;
        public static AutoResetEvent WhoIsControl = new AutoResetEvent(false);

        //创建明密状态变量，默认为明
        public static bool MingStatus = true;
        //创建密钥参数变量，默认为初始密钥
        public static bool MiYueStatus = true;
        //创建算法参数变量，默认为初始算法
        public static bool SuanFaStatus = true;
                

        //创建小回路比对应答事件
        public static ManualResetEvent WaitXHL_Return2ZK = new ManualResetEvent(false);
        public static byte ReturnCode = 0x00;
        //创建明密态全局变量
        public static bool MingMiTag = false;

        public static ManualResetEvent ServerConnectEvent = new ManualResetEvent(false);
        public static ManualResetEvent ServerConnectEvent2 = new ManualResetEvent(false);
        public struct CRT_STRUCT
        {
            public bool XHLEnable;
            public String CRTName;
            public PictureBox Led;

            public int TCMsgStatus;//返回码
            public TextBox mytextbox;//显示返回码
            public string Transfer2CRTa_TempStr;
            public Queue<byte[]> DataQueue_CRT;
            public bool CompareXHLResult;
            public int SendCount;//给CRT发送次数
            public int SendKB;//给CRT发送数据量
            public TextBox mytextbox_count;//显示发送次数
            public TextBox mytextbox_KB;//显示数据量

            public void LedOn()
            {
                this.Led.Image = Properties.Resources.green;
            }
            public void LedOff()
            {
                this.Led.Image = Properties.Resources.red;
            }
            public void init()
            {
                this.SendCount = 0;
                this.SendKB = 0;
                this.TCMsgStatus = 0;
                this.Transfer2CRTa_TempStr = "";
                this.LedOff();
                DataQueue_CRT = new Queue<byte[]>();
                CompareXHLResult = false;
            }
        }
        public static CRT_STRUCT DealCRTa = new CRT_STRUCT();
        public static CRT_STRUCT DealCRTb = new CRT_STRUCT();
        public static CRT_STRUCT DealZSSA = new CRT_STRUCT();
        public static CRT_STRUCT DealKSSA = new CRT_STRUCT();
        
        //public static Queue<byte[]> DataQueue_CRTa = new Queue<byte[]>();   //用于转发给USB应答机a,即CRTa
        //public static Queue<byte[]> DataQueue_CRTb = new Queue<byte[]>();   //用于转发给USB应答机b,即CRTb
        //public static Queue<byte[]> DataQueue_ZSSA = new Queue<byte[]>();   //用于转发给窄波束SSA,即ZSSA
        //public static Queue<byte[]> DataQueue_KSSA = new Queue<byte[]>();   //用于转发给宽波束SSA,即KSSA
        public static Queue<byte[]> DataQueue_GT = new Queue<byte[]>();       //用于转发给高通地测，即KSA中继
        public static Queue<byte[]> DataQueue_ZK_ACK = new Queue<byte[]>();        //用于给总控发送应答帧

        //------------------------IP地址及端口号--------------------------
        public static string ZK_IP_Z = "10.65.33.1";//总控服务器IP地址(主)
        public static int ZK_PORT_Z = 5001;

        public static string ZK_IP_B = "10.65.33.2";//总控服务器IP地址(备)
        public static int ZK_PORT_B = 5002;

        public static string ExternNet_IP = "10.65.33.161";//外系统接口计算机IP地址
        public static int ExternNet_PORT = 5175;

        //public static string KERNAL_IP = "10.65.33.175";//核心舱模拟器IP地址
        //public static int KERNAL_PORT_B = 5175;

        //public static string GT_IP = "192.168.0.15";//高通IP地址
        //public static int GT_PORT = 9000;
        //public static string LOCAL_IP = "192.168.0.5";//与高通连接的本地IP地址
        //public static int LOCAL_PORT = 9000;

        //----------------------------航天器编号---------------------------------------

        public static byte[] Num_MTC = new byte[8] { (byte)'T', (byte)'G', (byte)'M',
                      (byte)'T', (byte)'C',(byte) '1',(byte)'0', (byte)'1' };//梦天初样TGMTC001
        public static byte[] Num_X07 = new byte[8] { (byte)'X', (byte)'0', (byte)'7',
                      (byte)' ', (byte)' ',(byte) ' ',(byte)' ', (byte)' ' };//X07     

        //--------------------------数据标识-----------------------------------
        public static byte Data_Flag_Real = (byte)'R';//实时
        public static byte Data_Flag_Replay = (byte)'A';//回放

        //--------------------------信息标识-----------------------------------
        public static byte[] InfoFlag_Login = new byte[4] { (byte)'L', (byte)'O', (byte)'G',(byte)'N' };//签到信息
        public static byte[] InfoFlag_Time = new byte[4] { (byte)'U', (byte)'C', (byte)'L', (byte)'K' };//校时信息
        public static byte[] InfoFlag_Set = new byte[4] { (byte)'S', (byte)'E', (byte)'T', (byte)'P' };//地面设备设置命令
        public static byte[] InfoFlag_Stat = new byte[4] { (byte)'D', (byte)'A', (byte)'T', (byte)'S' };//地面设备状态信息
        public static byte[] InfoFlag_SetOk = new byte[4] { (byte)'S', (byte)'A', (byte)'C', (byte)'K' };//地面设备设置命令应答

        public static byte[] InfoFlag_CACK = new byte[4] { (byte)'C', (byte)'A', (byte)'C', (byte)'K' };//遥控指令应答
        public static byte[] InfoFlag_KACK = new byte[4] { (byte)'K', (byte)'A', (byte)'C', (byte)'K' };//遥控注数应答
        public static byte[] InfoFlag_ACKR = new byte[4] { (byte)'A', (byte)'C', (byte)'K', (byte)'R' };//小回路比对应答
        public static byte[] InfoFlag_DAGF = new byte[4] { (byte)'D', (byte)'A', (byte)'G', (byte)'F' };
        public static byte[] InfoFlag_DCUZ = new byte[4] { (byte)'D', (byte)'C', (byte)'U', (byte)'Z' };//对地测控通道下行遥测源码
        public static byte[] InfoFlag_DMTC = new byte[4] { (byte)'D', (byte)'M', (byte)'Y', (byte)'C' };//对地测控上行注数数据
        //-------------------------辅助标识------------------------------------
        public static byte Help_Flag = (byte)':';

        //----------------------信息来源/目的地址名称--------------------------
        public static byte[] ZK_S1 = new byte[3] { (byte)'M', (byte)'S', (byte)'1'};//总控服务器（主）
        public static byte[] ZK_S2 = new byte[3] { (byte)'M', (byte)'S', (byte)'2' };//总控服务器（备）
        public static byte[] TMF = new byte[3] { (byte)'T', (byte)'M', (byte)'F' };//遥测前端设备
        public static byte[] TCF = new byte[3] { (byte)'T', (byte)'C', (byte)'F' };//遥控前端设备
        public static byte[] IPC = new byte[3] { (byte)'I', (byte)'P', (byte)'C' };//外系统接口计算机
        
        public static byte[] IMAGE = new byte[3] { (byte)'V', (byte)'I', (byte)'D' };//图像处理设备
        public static byte[] GT    = new byte[3] { (byte)'G', (byte)'H', (byte)'E' };//高通
        public static byte[] KKT   = new byte[3] { (byte)'K', (byte)'K', (byte)'T' };//空空通信模拟器        
    }
}

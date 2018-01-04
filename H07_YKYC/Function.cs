using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace H07_YKYC
{
    class Function
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort vYear;
            public ushort vMonth;
            public ushort vDayofWeek;
            public ushort vDay;
            public ushort vHour;
            public ushort vMinute;
            public ushort vSecond;
            public ushort vMilliseconds;
        }
        //-----------------------------------------------
        [DllImport("Kernel32.dll")]
        public extern static void GetLocalTime(ref SystemTime lpSystemTime);
        [DllImport("Kernel32.dll")]
        public extern static void SetLocalTime(ref SystemTime lpSystemTime);
        //===============================================


        public static string Get_KcmdText(int j)
        {
            byte[] temp = new byte[5];
            byte[] ch = new byte[9];

            try
            {

                temp[0] = Data.KcmdText[4 * (j - 1)];
                temp[1] = Data.KcmdText[4 * (j - 1) + 1];
                temp[2] = Data.KcmdText[4 * (j - 1) + 2];
                temp[3] = Data.KcmdText[4 * (j - 1) + 3];
            }
            catch (Exception ex)
            {
                MyLog.Error("执行本地发送K令失败,未加载K令码表" + ex.Message);
                return "Error";
            }
            ch[0] = temp[0];
            ch[1] = (byte)((temp[1] & 0xf0) + ((temp[2] & 0xf0) >> 4));
            ch[2] = (byte)(((temp[2] & 0x0f) << 4) + ((temp[3] & 0xf0) >> 4));
            ch[3] = ch[0];
            ch[4] = ch[1];
            ch[5] = ch[2];
            ch[6] = ch[0];
            ch[7] = ch[1];
            ch[8] = ch[2];

            string KcmdStr = null;

            for (int i = 0; i < 9; i++)
            {
                KcmdStr += ch[i].ToString("x2");
            }

            return KcmdStr;
        }



        //----------组织发送给高通地测的数据包-------------------------------
        public static byte[] Make_toGT_frame(byte[] data, String type_str)
        {
            UInt16 len = (UInt16)(data.Length + 27);
            byte[] time_login = new byte[6];

            byte[] data_toGT = new byte[data.Length + 29];

            data_toGT[0] = (byte)(len & 0x00FF);
            data_toGT[1] = (byte)((len & 0xFF00) >> 8);//长度

            Data.Num_MTC.CopyTo(data_toGT, 2);//航天器编号:TGMTC001

            time_login = Function.Get_Time();
            time_login.CopyTo(data_toGT, 11);//时间

            data_toGT[17] = (byte)'R';//参数:数据标识:R

            data_toGT[18] = (byte)'Y';
            data_toGT[19] = (byte)'2';
            data_toGT[20] = (byte)'G';

            if (type_str == "CDBK") data_toGT[21] = (byte)'3';
            if (type_str == "CDKY") data_toGT[21] = (byte)'2';

            data_toGT[22] = Data.Help_Flag;//辅助标识::

            data_toGT[23] = (byte)'T';
            data_toGT[24] = (byte)'C';
            data_toGT[25] = (byte)'F';
            data_toGT[26] = (byte)'G';
            data_toGT[27] = (byte)'H';
            data_toGT[28] = (byte)'E';

            data.CopyTo(data_toGT, 29);//参数:信息体

            return data_toGT;
        }

        public static byte[] Make_toGT_frame_Y2G3(byte[] data)
        {
            UInt16 len = (UInt16)(836 + 27);
            byte[] time_login = new byte[6];

            byte[] data_toGT = new byte[836 + 29];

            data_toGT[0] = (byte)(len & 0x00FF);
            data_toGT[1] = (byte)((len & 0xFF00) >> 8);//长度

            Data.Num_MTC.CopyTo(data_toGT, 2);//航天器编号:TGMTC001

            time_login = Function.Get_Time();
            time_login.CopyTo(data_toGT, 11);//时间

            data_toGT[17] = (byte)'R';//参数:数据标识:R

            data_toGT[18] = (byte)'Y';
            data_toGT[19] = (byte)'2';
            data_toGT[20] = (byte)'G';
            data_toGT[21] = (byte)'3';

            data_toGT[22] = Data.Help_Flag;//辅助标识::

            data_toGT[23] = (byte)'T';
            data_toGT[24] = (byte)'C';
            data_toGT[25] = (byte)'F';
            data_toGT[26] = (byte)'G';
            data_toGT[27] = (byte)'H';
            data_toGT[28] = (byte)'E';

            byte[] temp = new byte[836];
            Array.Copy(data, 17, temp, 0, 836);
            temp.CopyTo(data_toGT, 29);

            return data_toGT;
        }

        public static byte[] Make_toGT_frame_Y2G2(byte[] data)
        {
            UInt16 len = (UInt16)(12 + 27);
            byte[] time_login = new byte[6];

            byte[] data_toGT = new byte[12 + 29];

            data_toGT[0] = (byte)(len & 0x00FF);
            data_toGT[1] = (byte)((len & 0xFF00) >> 8);//长度

            Data.Num_MTC.CopyTo(data_toGT, 2);//航天器编号:TGMTC001

            time_login = Function.Get_Time();
            time_login.CopyTo(data_toGT, 11);//时间

            data_toGT[17] = (byte)'R';//参数:数据标识:R

            data_toGT[18] = (byte)'Y';
            data_toGT[19] = (byte)'2';
            data_toGT[20] = (byte)'G';
            data_toGT[21] = (byte)'2';

            data_toGT[22] = Data.Help_Flag;//辅助标识::

            data_toGT[23] = (byte)'T';
            data_toGT[24] = (byte)'C';
            data_toGT[25] = (byte)'F';
            data_toGT[26] = (byte)'G';
            data_toGT[27] = (byte)'H';
            data_toGT[28] = (byte)'E';

            byte[] temp = new byte[12];
            Array.Copy(data, 4, temp, 0, 12);
            temp.CopyTo(data_toGT, 29);

            return data_toGT;
        }

        public static byte[] Make_toGT_frame_Y2G1(byte[] data)
        {
            Trace.WriteLine("进入Make_toGT_frame_Y2G1");
            //信息体
            byte[] CmdType = new byte[4];                               //指令类型
            byte[] CmdNum = new byte[12];                               //指令代号
            byte DataTag;                                               //数据标识
            byte[] DataStruct = new byte[data.Length - 17];             //数据体
            Array.Copy(data, 17, DataStruct, 0, data.Length - 17);

            int lenToGT = 30;

            int count = 0;
            List<int> LocateList = new List<int>();
            LocateList.Add(0);

            if (DataStruct[0] == (byte)'K')
                lenToGT += 71;
            if (DataStruct[0] == (byte)'M')
                lenToGT += 516;
            if (DataStruct[0] == (byte)'Z')
                lenToGT += 516;

            for (int t = 0; t < DataStruct.Length; t++)
            {
                if (DataStruct[t] == (byte)';')
                {
                    if (DataStruct[t + 1] == (byte)'K')
                    {
                        count += 1;
                        LocateList.Add(t + 1);
                        lenToGT += 71;
                    }
                    if (DataStruct[t + 1] == (byte)'M')
                    {
                        count += 1;
                        LocateList.Add(t + 1);
                        lenToGT += 516;
                    }
                    if (DataStruct[t + 1] == (byte)'Z')
                    {
                        count += 1;
                        LocateList.Add(t + 1);
                        lenToGT += 516;
                    }
                }
            }

            UInt16 len = (UInt16)(data.Length + 27);
            byte[] time_login = new byte[6];

            byte[] data_toGT = new byte[lenToGT];

            data_toGT[0] = (byte)(len & 0x00FF);
            data_toGT[1] = (byte)((len & 0xFF00) >> 8);//长度

            Data.Num_MTC.CopyTo(data_toGT, 2);//航天器编号:TGMTC001

            time_login = Function.Get_Time();
            time_login.CopyTo(data_toGT, 11);//时间

            data_toGT[17] = (byte)'R';//参数:数据标识:R

            data_toGT[18] = (byte)'Y';
            data_toGT[19] = (byte)'2';
            data_toGT[20] = (byte)'G';
            data_toGT[21] = (byte)'1';

            data_toGT[22] = Data.Help_Flag;//辅助标识::

            data_toGT[23] = (byte)'T';
            data_toGT[24] = (byte)'C';
            data_toGT[25] = (byte)'F';
            data_toGT[26] = (byte)'G';
            data_toGT[27] = (byte)'H';
            data_toGT[28] = (byte)'E';

            data_toGT[29] = (byte)(count + 1);

            int Tag = 30;//

            for (int i = 0; i < count + 1; i++)
            {
                switch (DataStruct[LocateList[i]])
                {
                    case (byte)'K':

                        byte[] EPDU_K = new byte[71];
                        EPDU_K[0] = 0xEB;
                        EPDU_K[1] = 0x90;

                        byte[] temp = new byte[67];
                        temp[0] = 0x41;
                        temp[1] = 0xCD;
                        temp[2] = 0xCC;         //明指令
                        //temp[2] = 0x33;          //密指令
                        byte[] temp9B = new byte[9] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
                        temp9B.CopyTo(temp, 3);
                        byte[] temp55B = new byte[55];
                        for (int j = 0; j < 55; j++) temp55B[j] = 0xAA;
                        temp55B.CopyTo(temp, 12);

                        temp.CopyTo(EPDU_K, 2);
                        EPDU_K[69] = 0x09;
                        EPDU_K[70] = 0xD7;

                        EPDU_K.CopyTo(data_toGT, Tag);
                        Tag += 71;

                        break;
                    case (byte)'M':
                        byte[] EPDU_M = new byte[516];
                        EPDU_M[0] = 0xEB;
                        EPDU_M[1] = 0x90;
                        byte[] tempM = new byte[512];
                        tempM[0] = 0x41;
                        tempM[1] = 0xCD;
                        tempM[2] = 0x66;
                        for (int m = 3; m < 512; m++) tempM[m] = 0x55;

                        tempM.CopyTo(EPDU_M, 2);

                        EPDU_M[514] = 0x09;
                        EPDU_M[515] = 0xD7;


                        EPDU_M.CopyTo(data_toGT, Tag);
                        Tag += 516;

                        break;
                    case (byte)'Z':
                        byte[] EPDU_Z = new byte[516];

                        Array.Copy(DataStruct, LocateList[i], EPDU_Z, 0, 516);

                        EPDU_Z.CopyTo(data_toGT, Tag);
                        Tag += 516;

                        break;
                }

            }
            //    data.CopyTo(data_toGT, 29);//参数:信息体

            return data_toGT;
        }

        //----------组织发送给CRT应答机的数据包-------------------------------
        public static byte[] Make_toCRT_frame(byte[] data, bool Tag)
        {
            byte[] data_toCRT = new byte[71];
            byte[] JMXL = new byte[32];
            byte[] ADDXL = new byte[23];

            if (Tag)
            {
                for (int i = 0; i < 32; i++) JMXL[i] = 0xAA;
            }
            else
            {
                for (int i = 0; i < 32; i++) JMXL[i] = 0x55;
            }
            for (int i = 0; i < 23; i++) ADDXL[i] = 0xAA;

            //启动序列EB90H
            data_toCRT[0] = 0xeb;
            data_toCRT[1] = 0x90;
            //地址同步字41CDH表示实验舱2
            data_toCRT[2] = 0x41;
            data_toCRT[3] = 0xCD;
            //方式字
            if (Tag)
            {
                data_toCRT[4] = 0xCC;

            }
            else
            {
                data_toCRT[4] = 0x33;
            }
            //指令码
            data.CopyTo(data_toCRT, 5);
            //加密认证序列
            JMXL.CopyTo(data_toCRT, 14);
            //填充序列
            ADDXL.CopyTo(data_toCRT, 46);
            //结束序列09D7
            data_toCRT[69] = 0x09;
            data_toCRT[70] = 0xD7;

            return data_toCRT;
        }


        public static byte[] Make_toCortex_frame(byte[] data)
        {
            byte[] data_toCRT;

            //Trace.WriteLine(data.Length);
            int AddZero = 4 - data.Length % 4;
            byte[] FinalData = new byte[data.Length + AddZero];
            //Trace.WriteLine(FinalData.Length);

            for (int m = 0; m < FinalData.Length; m++) FinalData[m] = 0;
            Array.Copy(data, 0, FinalData, 0, data.Length);

            int N = 8 + FinalData.Length / 4;

            //send request
            int[] SendReq = new int[N];
            SendReq[0] = 1234567890;        //Standard tcp-ip header 0
                                            //SendReq[1] = 64;                //Standard tcp-ip header 1 Total Bytes;
            SendReq[1] = 32 + FinalData.Length;
            SendReq[2] = 0;                 //Standard tcp-ip header 2
            SendReq[3] = 1;                 //Request code
            SendReq[4] = 0;                 //Command tag
                                            //           SendReq[5] = 0;                 //Message length
            SendReq[5] = (data.Length) * 8;

            int[] frameContent = Program.BytesToInt(FinalData);
            Array.Copy(frameContent, 0, SendReq, 6, frameContent.Length);

            SendReq[N - 2] = 0;              //Check-Sum
            SendReq[N - 1] = -1234567890;     //Standard tcp-ip postamble

            int crc = 0;
            for (int j = 0; j < N; j++)
            {
                crc += SendReq[j];
            }

            SendReq[N - 2] = 0 - crc;

            data_toCRT = Program.IntToBytes(SendReq);



            return data_toCRT;
        }

        //----------组织至总控数据包--------------遥控应答----
        public static byte[] Make_tozk_frame(byte data_flag, byte[] Info_flag, byte[] data)
        {
            UInt16 len = (UInt16)(data.Length + 27);
            byte[] data_tozk = new byte[data.Length + 29];
            byte[] time_login = new byte[6];

            data_tozk[0] = (byte)(len & 0x00FF);
            data_tozk[1] = (byte)((len & 0xFF00) >> 8);//长度

            Data.Num_X07.CopyTo(data_tozk, 2);//航天器编号:TGMTC001

            time_login = Function.Get_Time();
            time_login.CopyTo(data_tozk, 11);//时间
            data_tozk[16] = 0x00;
            data_tozk[17] = data_flag;//参数:数据标识:R

            //信息标识:CACK、KACK、ACKR
            Info_flag.CopyTo(data_tozk, 18);

            data_tozk[22] = Data.Help_Flag;//辅助标识::

            Data.TCF.CopyTo(data_tozk, 23);//信息来源:TCF
            Data.ZK_S1.CopyTo(data_tozk, 26);//参数:信息目的


            //===========================================
            //信息体加入数组
            //===========================================
            data.CopyTo(data_tozk, 29);//参数:信息体

            return data_tozk;
        }

        //----------组织至总控数据包--------------遥测数据----
        public static byte[] Make_tozk_YC_frame(byte data_flag, byte[] Info_flag, byte[] data)
        {
            UInt16 len = (UInt16)(data.Length + 27);
            byte[] data_tozk = new byte[data.Length + 29];
            byte[] time_login = new byte[6];

            data_tozk[0] = (byte)(len & 0x00FF);
            data_tozk[1] = (byte)((len & 0xFF00) >> 8);//长度

            Data.Num_X07.CopyTo(data_tozk, 2);//航天器编号:X07     

            time_login = Function.Get_Time();
            time_login.CopyTo(data_tozk, 11);//时间
            data_tozk[16] = 0x00;
            data_tozk[17] = data_flag;//参数:数据标识:R

            //信息标识:DAGF
            Info_flag.CopyTo(data_tozk, 18);

            data_tozk[22] = Data.Help_Flag;//辅助标识::

            Data.TMF.CopyTo(data_tozk, 23);//信息来源:TMF
            Data.ZK_S1.CopyTo(data_tozk, 26);//参数:信息目的


            //===========================================
            //信息体加入数组--VCDU(1018Byte遥测源码)
            //===========================================
            data.CopyTo(data_tozk, 29);//参数:信息体

            return data_tozk;
        }

        //----------组织签到包--------------
        public static byte[] Make_login_frame(byte data_flag, byte[] dest)
        {
            UInt16 len = 29;
            byte[] data_login = new byte[31];
            byte[] time_login = new byte[6];

            data_login[0] = (byte)(len & 0x00FF);
            data_login[1] = (byte)((len & 0xFF00) >> 8);//长度[0~1]

            Data.Num_MTC.CopyTo(data_login, 2);//航天器编号[2~9]“梦天初样”

            time_login = Function.Get_Time();
            time_login.CopyTo(data_login, 10);//时间[10~15]

            //保留[16]
            data_login[16] = 0x00;
            data_login[17] = data_flag;//参数:数据标识[17]“R”

            Data.InfoFlag_Login.CopyTo(data_login, 18);//信息标识[18~21]“LOGN”

            data_login[22] = Data.Help_Flag;//辅助标识[22]":"

            Data.TCF.CopyTo(data_login, 23);//信息来源[23~25]“遥测前端设备”

            dest.CopyTo(data_login, 26);//参数:信息目的[26~28]

            data_login[29] = (byte)'O';
            data_login[30] = (byte)'N';//信息体

            return data_login;
        }
        //---------获取系统时间---------
        public static byte[] Get_Time()
        {
            byte[] time_span = new byte[6];

            DateTime nowtime = DateTime.Now;
            DateTime start_time = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan ts = nowtime - start_time;

            UInt16 days = (UInt16)ts.Days;
            UInt32 ms = (UInt32)(ts.TotalMilliseconds - ts.Days * 24 * 60 * 60 * 1000) * 10;//??

            time_span[0] = (byte)(days & 0x00FF);
            time_span[1] = (byte)((days & 0xFF00) >> 8);
            time_span[2] = (byte)(ms & 0x000000FF);
            time_span[3] = (byte)((ms & 0x0000FF00) >> 8);
            time_span[4] = (byte)((ms & 0x00FF0000) >> 16);
            time_span[5] = (byte)((ms & 0xFF000000) >> 24);

            return time_span;
        }
        //----------设置系统时间----------
        public static void Set_Time(byte[] data)
        {
            try
            {
                byte[] gettime = new byte[23];
                for (int datai = 0; datai < data.Length; datai++)
                {
                    gettime[datai] = byte.Parse(data[datai].ToString());
                }
                string dt_str = Encoding.ASCII.GetString(gettime);

                string str = dt_str.Substring(0, 4);
                SystemTime systime = new SystemTime();
                systime.vYear = ushort.Parse(str);

                str = dt_str.Substring(5, 2);
                systime.vMonth = ushort.Parse(str);

                str = dt_str.Substring(8, 2);
                systime.vDay = ushort.Parse(str);

                str = dt_str.Substring(11, 2);
                systime.vHour = ushort.Parse(str);

                str = dt_str.Substring(14, 2);
                systime.vMinute = ushort.Parse(str);

                str = dt_str.Substring(17, 2);
                systime.vSecond = ushort.Parse(str);

                str = dt_str.Substring(20, 3);
                systime.vMilliseconds = ushort.Parse(str);
                //--------更改系统时间--------
                SetLocalTime(ref systime);
            }
            catch
            {
                MyLog.Error("设置系统时间失败！");
            }
        }

        public static DateTime BytesToDateTime(byte[] bytes, int offset)
        {
            if (bytes != null)
            {
                long ticks = BitConverter.ToInt64(bytes, offset);
                if (ticks < DateTime.MaxValue.Ticks && ticks > DateTime.MinValue.Ticks)
                {
                    DateTime dt = new DateTime(ticks);
                    return dt;
                }
            }
            return new DateTime();
        }

        /// <summary>
        /// 十六进制String转化为BYTE数组
        /// </summary>
        /// <param name="hexString">参数：输入的十六进制String</param>
        /// <returns>BYTE数组</returns>
        public static byte[] StrToHexByte(string hexString)
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
        /// 在收到总控的K令数据包时，根据K令号获得K令实际内容
        /// </summary>
        /// <param name="j"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string Get_KcmdText(int j, ref byte[] ch)
        {
            byte[] temp = new byte[5];
            try
            {
                temp[0] = Data.KcmdText[4 * (j - 1)];
                temp[1] = Data.KcmdText[4 * (j - 1) + 1];
                temp[2] = Data.KcmdText[4 * (j - 1) + 2];
                temp[3] = Data.KcmdText[4 * (j - 1) + 3];
            }
            catch (Exception ex)
            {
                MyLog.Error("执行本地发送K令失败,未加载K令码表" + ex.Message);
                return "Error";
            }
            ch[0] = temp[0];
            ch[1] = (byte)((temp[1] & 0xf0) + ((temp[2] & 0xf0) >> 4));
            ch[2] = (byte)(((temp[2] & 0x0f) << 4) + ((temp[3] & 0xf0) >> 4));
            ch[3] = ch[0];
            ch[4] = ch[1];
            ch[5] = ch[2];
            ch[6] = ch[0];
            ch[7] = ch[1];
            ch[8] = ch[2];

            string KcmdStr = null;
            for (int i = 0; i < 9; i++)
            {
                KcmdStr += ch[i].ToString("x2");
            }
            return KcmdStr;
        }


        /// <summary>
        /// CRC16
        /// </summary>
        /// <param name="data"></param>
        /// <param name="genpoly"></param>
        /// <param name="crc"></param>
        /// <returns></returns>
        public static ushort CRChware(ushort data, ushort genpoly, ushort crc)
        {
            int i = 8;
            data <<= 8;
            for (i = 8; i > 0; i--)
            {
                if (((data ^ crc) & 0x8000) != 0)
                {
                    crc = (ushort)((crc << 1) ^ genpoly);
                }
                else
                {
                    crc <<= 1;
                }
                data <<= 1;
            }
            return crc;
        }

    }
}

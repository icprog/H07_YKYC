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
    public class SaveFile
    {

        FileStream file_out1, file_out2, file_out3, file_out4, file_out5, file_out6, file_out7, file_out8;
        List<FileStream> myFileList_dat = new List<FileStream>();

        FileStream file_in1, file_in2, file_in3, file_in4;
        List<FileStream> myFileList_txt = new List<FileStream>();


        public static ReaderWriterLockSlim Lock_Dat1 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat2 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat3 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat4 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat5 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat6 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat7 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat8 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Dat9 = new ReaderWriterLockSlim();

        public static List<ReaderWriterLockSlim> myLockforDat = new List<ReaderWriterLockSlim>();

        public static Queue<byte[]> DataQueue_out1 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out2 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out3 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out4 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out5 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out6 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out7 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out8 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_out9 = new Queue<byte[]>();

        public static List<Queue<byte[]>> DataQueue_outList = new List<Queue<byte[]>>();




        public static ReaderWriterLockSlim Lock_Txt1 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Txt2 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Txt3 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_Txt4 = new ReaderWriterLockSlim();
  
        public static List<ReaderWriterLockSlim> myLockforTxt = new List<ReaderWriterLockSlim>();

        public static Queue<string> DataQueue_in1 = new Queue<string>();
        public static Queue<string> DataQueue_in2 = new Queue<string>();
        public static Queue<string> DataQueue_in3 = new Queue<string>();
        public static Queue<string> DataQueue_in4 = new Queue<string>();

        public static List<Queue<string>> DataQueue_inList = new List<Queue<string>>();

        public static Queue<byte[]> DataQueue_Async = new Queue<byte[]>();

        //在网络收发线程中给DataQueue赋值

        public static Queue<byte[]> DataQueue_0 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_1 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_2 = new Queue<byte[]>();
        public static Queue<byte[]> DataQueue_3 = new Queue<byte[]>();

        public static bool SaveOn = true;

        //在主页面初始化时，调用FileInit()和FileSaveStart(),分别初始化文件夹和开始存储

        /// <summary>
        /// 存储线程初始化
        /// </summary>
        public void FileInit()
        {
            MyLog.Info("初始化运行日志存盘路径");
            Trace.WriteLine("Start FileInit!\n");

         //   FileCreateTxt(Program.GetStartupPath() + @"接收\总控设备（遥控）\", out file_in1);
            //FileCreateTxt(Program.GetStartupPath() + @"接收\总控设备（遥测）\", out file_in2);
          //  FileCreateTxt(Program.GetStartupPath() + @"接收\瑞信丰\", out file_in3);

            DataQueue_inList.Add(DataQueue_in1);
            DataQueue_inList.Add(DataQueue_in2);
            DataQueue_inList.Add(DataQueue_in3);

            myLockforTxt.Add(Lock_Txt1);
            myLockforTxt.Add(Lock_Txt2);
            myLockforTxt.Add(Lock_Txt3);


            FileCreateDat(Program.GetStartupPath() + @"接收\总控设备（遥控）\", out file_out1);
            FileCreateDat(Program.GetStartupPath() + @"发送\瑞信丰\", out file_out2);
            FileCreateDat(Program.GetStartupPath() + @"接收\瑞信丰\原始数据\", out file_out3);
            FileCreateDat(Program.GetStartupPath() + @"接收\瑞信丰\遥测数据\", out file_out4);
            FileCreateDat(Program.GetStartupPath() + @"发送\总控设备（遥测）\", out file_out5);
            FileCreateDat(Program.GetStartupPath() + @"生成数据\", out file_out6);
            //FileCreateDat(Program.GetStartupPath() + @"发送\其它舱（总控）\", out file_out7);

            DataQueue_outList.Add(DataQueue_out1);
            DataQueue_outList.Add(DataQueue_out2);
            DataQueue_outList.Add(DataQueue_out3);
            DataQueue_outList.Add(DataQueue_out4);
            DataQueue_outList.Add(DataQueue_out5);
            DataQueue_outList.Add(DataQueue_out6);
            DataQueue_outList.Add(DataQueue_out7);

            myLockforDat.Add(Lock_Dat1);
            myLockforDat.Add(Lock_Dat2);
            myLockforDat.Add(Lock_Dat3);
            myLockforDat.Add(Lock_Dat4);
            myLockforDat.Add(Lock_Dat5);
            myLockforDat.Add(Lock_Dat6);
            myLockforDat.Add(Lock_Dat7);

        }

        public void FileCreateDat(string Path, out FileStream file)
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            DirectoryInfo di = new DirectoryInfo(Path);
            foreach (FileInfo fi in di.GetFiles())
            {
                if(fi.Length==0)
                {
                    File.Delete(fi.FullName);
                }
            }

            string timestr = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string filename = Path + timestr + ".dat";
            file = new FileStream(filename, FileMode.Create);
            myFileList_dat.Add(file);
        }

        public void FileCreateTxt(string Path, out FileStream file)
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            DirectoryInfo di = new DirectoryInfo(Path);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Length == 0)
                {
                    File.Delete(fi.FullName);
                }
            }

            string timestr = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string filename = Path + timestr + ".txt";
            file = new FileStream(filename, FileMode.Create);
            myFileList_txt.Add(file);
        }

        public void FileSaveStart()
        {
            //       MyLog.Info("开启存盘线程！");
            Trace.WriteLine("开启存盘线程");
            SaveOn = true;

            new Thread(() => { WriteToFileDat(0,file_out1,ref DataQueue_out1,ref Lock_Dat1); }).Start();//接收总控-->遥控的数据Dat
            new Thread(() => { WriteToFileDat(1, file_out2, ref DataQueue_out2, ref Lock_Dat2); }).Start();//发送给瑞信丰的数据
            new Thread(() => { WriteToFileDat(2, file_out3, ref DataQueue_out3, ref Lock_Dat3); }).Start();//接收瑞信丰的原始数据
            new Thread(() => { WriteToFileDat(3, file_out4, ref DataQueue_out4, ref Lock_Dat4); }).Start();//接收瑞信丰的原始数据后去头去尾后的遥测数据

            new Thread(() => { WriteToFileDat(4, file_out5, ref DataQueue_out5, ref Lock_Dat5); }).Start();//发送-->总控的数据Dat
            new Thread(() => { WriteToFileDat(5, file_out6, ref DataQueue_out6, ref Lock_Dat6); }).Start();//发送给空空的数据
            //new Thread(() => { WriteToFileDat(6, file_out7, ref DataQueue_out7, ref Lock_Dat7); }).Start();//发送给其它舱（总控）的数据


    //        new Thread(() => { WriteToFileTxt(0, file_in1, ref DataQueue_in1, ref Lock_Txt1); }).Start();//接收总控-->遥控的数据txt
    //        new Thread(() => { WriteToFileTxt(1, file_in2, ref DataQueue_in2, ref Lock_Txt2); }).Start();//接收总控（备）的数据
    //        new Thread(() => { WriteToFileTxt(2, file_in3, ref DataQueue_in3, ref Lock_Txt3); }).Start();
        }

        public void FileClose()
        {
            MyLog.Info("关闭所有存盘线程");
            SaveOn = false;
            Trace.WriteLine("Start FileClose!\n");
            Thread.Sleep(500);

            foreach (var item in myFileList_dat)
            {
                item.Close();
            }

            foreach (var item in myFileList_txt)
            {
                item.Close();
            }

        }

        private void WriteToFileDat(int key, object file, ref Queue<byte[]> myQueue, ref ReaderWriterLockSlim myLock)
        {
            Trace.WriteLine("Start WriteToFileout Thread:" + key.ToString());
            FileStream myfile = (FileStream)file;
            BinaryWriter bw = new BinaryWriter(myfile);
            //     FileInfo fileInfo;

            while (SaveOn)
            {
                if (myQueue.Count() > 0)
                {
                    try
                    {
                        myLock.EnterReadLock();
                        bw.Write(myQueue.Dequeue());
                        bw.Flush();
                        myLock.ExitReadLock();

                        #region 分割文件，防止文件过大
                        long FileSizeMB = myfile.Length / (1024 * 1024 * 1024);
                        if (FileSizeMB > 1)
                        {
                            myFileList_dat[key].Flush();

                            string Path2 = myFileList_dat[key].Name;
                            int count = Path2.LastIndexOf("\\");
                            Path2 = Path2.Substring(0, count + 1);

                            myFileList_dat[key].Close();

                            FileStream newFile;
                            string timestr = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            string filename = Path2 + timestr + ".dat";
                            newFile = new FileStream(filename, FileMode.Create);

                            myFileList_dat.Remove(myFileList_dat[key]);
                            myFileList_dat.Insert(key, newFile);

                            break;
                            //break跳出循环会执行新线程
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(myQueue.Count());
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(200);
                    //                  Trace.WriteLine("Queue0 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFileSC:" + key.ToString());

            if (SaveOn) WriteToFileDat(key, myFileList_dat[key], ref myQueue, ref myLock);

        }


        private void WriteToFileTxt(int key, object file, ref Queue<string> myQueue, ref ReaderWriterLockSlim myLock)
        {
            Trace.WriteLine("Start WriteToFileAsync Thread:" + key.ToString());
            FileStream myfile = (FileStream)file;
            StreamWriter bw = new StreamWriter(myfile);
            //      FileInfo fileInfo;
            while (SaveOn)
            {
                if (myQueue.Count() > 0)
                {
                    try
                    {
                        myLock.EnterReadLock();
                        bw.Write(myQueue.Dequeue());
                        bw.Flush();
                        myLock.ExitReadLock();

                        #region 分割文件，防止文件过大
                        long FileSizeMB = myfile.Length / (1024 * 1024);
                        if (FileSizeMB > 10)
                        {
                            myFileList_txt[key].Flush();

                            string Path2 = myFileList_txt[key].Name;
                            int count = Path2.LastIndexOf("\\");
                            Path2 = Path2.Substring(0, count + 1);

                            myFileList_txt[key].Close();

                            FileStream newFile;
                            string timestr = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            string filename = Path2 + timestr + ".txt";
                            newFile = new FileStream(filename, FileMode.Create);

                            myFileList_txt.Remove(myFileList_txt[key]);
                            myFileList_txt.Insert(key, newFile);

                            break;
                            //break跳出循环会执行新线程
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(myQueue.Count());
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(200);
                    //                  Trace.WriteLine("Queue0 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFileAsync:" + key.ToString());

            if (SaveOn) WriteToFileTxt(key, myFileList_txt[key], ref myQueue, ref myLock);

        }

        private void WriteToFile0(object file)
        {
            Trace.WriteLine("Start WriteToFile Thread!\n");
            FileStream myfile = (FileStream)file;
            BinaryWriter bw = new BinaryWriter(myfile);

            while (SaveOn)
            {
                if (DataQueue_0.Count() > 0)
                {
                    try
                    {
                        //                     Trace.WriteLine("DataQueue_1 size is {0}!!", DataQueue_0.Count());
                        bw.Write(DataQueue_0.Dequeue());
                        bw.Flush();
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    //                  Trace.WriteLine("Queue0 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFile0!!");
        }

        private void WriteToFile1(object file)
        {
            Trace.WriteLine("Start WriteToFile Thread!\n");
            FileStream myfile = (FileStream)file;
            BinaryWriter bw = new BinaryWriter(myfile);

            while (SaveOn)
            {
                if (DataQueue_1.Count() > 0)
                {
                    try
                    {
                        //                 Trace.WriteLine("DataQueue_1 size is {0}!!", DataQueue_1.Count());
                        bw.Write(DataQueue_1.Dequeue());
                        bw.Flush();
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    //              Trace.WriteLine("Queue1 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFile1!!");
        }

        private void WriteToFile2(object file)
        {
            Trace.WriteLine("Start WriteToFile Thread!\n");
            FileStream myfile = (FileStream)file;
            BinaryWriter bw = new BinaryWriter(myfile);

            while (SaveOn)
            {
                if (DataQueue_2.Count() > 0)
                {
                    try
                    {
                        Trace.WriteLine("DataQueue_2 size is {0:d}!!", DataQueue_2.Count().ToString());
                        bw.Write(DataQueue_2.Dequeue());
                        bw.Flush();
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    //               Trace.WriteLine("Queue2 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFile!!");
        }

        private void WriteToFile3(object file)
        {
            Trace.WriteLine("Start WriteToFile Thread!\n");
            FileStream myfile = (FileStream)file;
            BinaryWriter bw = new BinaryWriter(myfile);

            while (SaveOn)
            {
                if (DataQueue_3.Count() > 0)
                {
                    try
                    {
                        Trace.WriteLine("DataQueue_3 size is {0}!!", DataQueue_3.Count().ToString());
                        bw.Write(DataQueue_3.Dequeue());
                        bw.Flush();
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    //              Trace.WriteLine("Queue3 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFile!!");
        }

    }
}

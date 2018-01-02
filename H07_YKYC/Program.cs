using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H07_YKYC
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        //获取当前执行文件路径
        public static string GetStartupPath()
        {
            string path;
            path = Application.StartupPath;
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }
            return path;
        }

        public static byte[] IntToBytes(int[] IntData)
        {
            byte[] ByteData = new byte[IntData.Length * 4];
            for (int i = 0; i < IntData.Length; i++)
            {
                ByteData[i * 4] = (byte)((IntData[i] >> 24) & 0xff);
                ByteData[i * 4 + 1] = (byte)((IntData[i] >> 16) & 0xff);
                ByteData[i * 4 + 2] = (byte)((IntData[i] >> 8) & 0xff);
                ByteData[i * 4 + 3] = (byte)((IntData[i] >> 0) & 0xff);
            }
            return ByteData;
        }

        public static int[] BytesToInt(byte[] ByteData)
        {
            int[] IntData = new int[ByteData.Length / 4];
            for (int i = 0; i < IntData.Length; i++)
            {
                IntData[i] = (((ByteData[i * 4] & 0xff) << 24) +
                    ((ByteData[i * 4 + 1] & 0xff) << 16) +
                    ((ByteData[i * 4 + 2] & 0xff) << 8) +
                    (ByteData[i * 4 + 3] & 0xff));
            }
            return IntData;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H07_YKYC
{
    public partial class XuLieProduce : Form
    {
        public MainForm mform;
        public XuLieProduce(H07_YKYC.MainForm parent)
        {
            InitializeComponent();
            mform = parent;
        }

        public int SeqNo;
        private void button1_Click(object sender, EventArgs e)
        {
            SeqNo = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] temp = new string[10];
            temp[1] = "发送指令";
            temp[2] = textBox1.Text;

            int Kcode = 0;
            if (int.TryParse(temp[2], out Kcode) & (Kcode > 0))
            {
                temp[3] = Function.Get_KcmdText(int.Parse(temp[2]));
                if (temp[3] == "Error")
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("输入正确的K令代号！");
                return;
            }

            temp[4] = textBox2.Text;//发送次数
            temp[5] = textBox3.Text;//发送间隔

            for (int i = 0; i < int.Parse(temp[4]); i++)
            {
                temp[0] = (SeqNo++).ToString();
                textBox4.AppendText(
                     "序号：" + temp[0] + "," +
                     "发送选项：" + temp[1] + "," +
                     "K令代号：" + temp[2] + "," +
                     "发送源码：" + temp[3] +
                     "发送间隔" + temp[5] + ";\n");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H07_YKYC
{
    public partial class LoginForm : Form
    {
        public MainForm mform;
        public LoginForm(H07_YKYC.MainForm parent)
        {
            InitializeComponent();
            mform = parent;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.label3.Visible = false;
            this.label4.Visible = false;
            this.textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "805H07_YKYC")
            {
                if (textBox2.Text == "123456")
                {
                    MyLog.Info("用户名密码通过校验,解锁成功");
                    mform.TagLock = 0;
                    mform.Logform_state = true;
                    this.Close();
                }
                else
                {
                    MyLog.Error("密码校验失败,解锁失败");
                    this.label3.Text = "密码错误";
                    this.label3.Visible = true;
                }
            }
            else
            {
                MyLog.Error("用户名校验失败,解锁失败");
                this.label4.Text = "用户名错误";
                this.label4.Visible = true;
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(mform.Logform_state!=true)
            {
                mform.Logform_state = true;
            }
        }
    }
}

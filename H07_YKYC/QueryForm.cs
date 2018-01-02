using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H07_YKYC
{
    public partial class QueryForm : Form
    {
        public MainForm mform;
        public QueryForm(H07_YKYC.MainForm parent)
        {
            InitializeComponent();
            mform = parent;
        }

        private void QueryForm_Load(object sender, EventArgs e)
        {
            DateTime timenow = System.DateTime.Now;
            String temp = timenow.ToString("yyyy-MM-dd hh:mm:ss");
            dateTimePicker2.Text = temp;
            DateTime timebefore = timenow.AddDays(-1);
            String tempbefore = timebefore.ToString("yyyy-MM-dd hh:mm:ss");
            dateTimePicker1.Text = tempbefore;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            switch (comboBox1.Text)
            {
                case "接收":
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("总控设备（主）");
                    comboBox2.Items.Add("总控设备（备）");
                    comboBox2.Items.Add("外系统接口计算机");
                    comboBox2.SelectedIndex = 0;
                    break;
                case "发送":
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("应答机a");
                    comboBox2.Items.Add("应答机b");
                    comboBox2.Items.Add("窄波束SSA");
                    comboBox2.Items.Add("宽波束SSA");
                    comboBox2.Items.Add("中继KSA");
                    comboBox2.Items.Add("空空");
                    comboBox2.Items.Add("其它舱（总控）");
                    comboBox2.SelectedIndex = 0;
                    break;
                case "小回路闭环":
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("应答机a");
                    comboBox2.Items.Add("应答机b");
                    comboBox2.Items.Add("窄波束SSA");
                    comboBox2.Items.Add("宽波束SSA");
                    comboBox2.SelectedIndex = 0;
                    break;
                default:
                    MyLog.Error("查询条件无效，无法查询文件");
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListViewItem item;
            ListViewItem.ListViewSubItem subitem1;
            ListViewItem.ListViewSubItem subitem2;

            listView1.Items.Clear();
            string path = Program.GetStartupPath() + comboBox1.Text + "\\" + comboBox2.Text + '\\';
            DirectoryInfo folder = new DirectoryInfo(path);

            try
            {
                foreach (FileInfo file in folder.GetFiles("*.*"))
                {
                    item = new ListViewItem();
                    item.Text = file.Name;
                    subitem1 = new ListViewItem.ListViewSubItem();
                    subitem1.Text = file.FullName;
                    item.SubItems.Add(subitem1);
                    subitem2 = new ListViewItem.ListViewSubItem();
                    subitem2.Text = file.Length.ToString();
                    item.SubItems.Add(subitem2);
                    listView1.Items.Add(item);
                }
            }
            catch(Exception ex)
            {
                MyLog.Error(ex.Message);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            System.Windows.Forms.ListView lw = (System.Windows.Forms.ListView)sender;

            try
            {
                string filename = lw.FocusedItem.SubItems[1].Text;
                System.Diagnostics.Process.Start(filename);
            }
            catch
            {
                MyLog.Error("openfile err");
                //textBox.Text += "openfile err\r\n";
                return;
            }
        }
    }
}

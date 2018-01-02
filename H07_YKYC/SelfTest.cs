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
    public partial class SelfTest : Form
    {
        public MainForm mform;
        public SelfTest(H07_YKYC.MainForm parent)
        {
            InitializeComponent();
            mform = parent;
        }

        private void SelfTest_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine("here is MainForm_Paint!!!");
            Pen mypen = new Pen(Color.Red);
            mypen.Width = 2;
            mypen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            this.panel1.CreateGraphics().DrawLine(mypen, panel4.Location.X + panel4.Width, panel4.Location.Y + panel4.Height / 2,
                panel6.Location.X, panel6.Location.Y + panel6.Height / 2);
            draw_arrow(panel1, mypen, panel6.Location.X, panel6.Location.Y + panel6.Height / 2, 0);
            this.panel1.CreateGraphics().DrawLine(mypen, panel4.Location.X + panel4.Width/2, panel4.Location.Y + panel4.Height,
                panel4.Location.X + panel4.Width / 2, panel3.Location.Y+panel3.Height/2);
            this.panel1.CreateGraphics().DrawLine(mypen,panel4.Location.X + panel4.Width / 2, panel3.Location.Y + panel3.Height / 2,
                panel3.Location.X, panel3.Location.Y + panel3.Height / 2);
            draw_arrow(panel1, mypen, panel3.Location.X, panel3.Location.Y + panel3.Height / 2, 0);


            e.Graphics.DrawLine(mypen, panel1.Location.X+panel1.Width,panel1.Location.Y+panel1.Height/2,
               panel1.Location.X + panel1.Width+10, panel1.Location.Y + panel1.Height / 2);
            e.Graphics.DrawLine(mypen,panel1.Location.X + panel1.Width + 10, panel8.Location.Y + panel8.Height / 2,
                panel8.Location.X, panel8.Location.Y + panel8.Height / 2);
            e.Graphics.DrawLine(mypen, panel1.Location.X + panel1.Width + 10, panel10.Location.Y + panel10.Height / 2,
                panel10.Location.X, panel10.Location.Y + panel10.Height / 2);
            e.Graphics.DrawLine(mypen, panel1.Location.X + panel1.Width + 10, panel14.Location.Y + panel14.Height / 2,
                panel14.Location.X, panel14.Location.Y + panel14.Height / 2);
            draw_arrow2(e, mypen, panel8.Location.X, panel8.Location.Y + panel8.Height / 2, 0);
            draw_arrow2(e, mypen, panel10.Location.X, panel10.Location.Y + panel10.Height / 2,0);
            draw_arrow2(e, mypen, panel14.Location.X, panel14.Location.Y + panel14.Height / 2, 0);
            e.Graphics.DrawLine(mypen, panel1.Location.X + panel1.Width + 10, panel8.Location.Y + panel8.Height / 2,
                panel1.Location.X + panel1.Width + 10, panel14.Location.Y + panel14.Height / 2);

            Pen mypen2 = new Pen(Color.Green);
            mypen2.Width = 2;
            mypen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            e.Graphics.DrawLine(mypen2, panel1.Location.X + panel1.Width, panel1.Location.Y + panel1.Height / 2+10,
                panel1.Location.X + panel1.Width + 30, panel1.Location.Y + panel1.Height / 2+10);
            e.Graphics.DrawLine(mypen2, panel1.Location.X + panel1.Width + 30, panel8.Location.Y + panel8.Height / 2+10,
                panel8.Location.X, panel8.Location.Y + panel8.Height / 2+10);
            e.Graphics.DrawLine(mypen2, panel1.Location.X + panel1.Width + 30, panel10.Location.Y + panel10.Height / 2+10,
                panel10.Location.X, panel10.Location.Y + panel10.Height / 2+10);
            e.Graphics.DrawLine(mypen2, panel1.Location.X + panel1.Width + 30, panel14.Location.Y + panel14.Height / 2+10,
                panel14.Location.X, panel14.Location.Y + panel14.Height / 2+10);
            e.Graphics.DrawLine(mypen2, panel1.Location.X + panel1.Width + 30, panel8.Location.Y + panel8.Height / 2+10,
                panel1.Location.X + panel1.Width + 30, panel14.Location.Y + panel14.Height / 2+10);
            draw_arrow2(e, mypen2, panel1.Location.X + panel1.Width, panel1.Location.Y + panel1.Height / 2 + 10, 1);

            this.panel1.CreateGraphics().DrawLine(mypen2, panel6.Location.X+panel6.Width/2, panel6.Location.Y + panel6.Height,
               panel6.Location.X + panel6.Width / 2,panel3.Location.Y + panel3.Height / 2);
            this.panel1.CreateGraphics().DrawLine(mypen2, panel6.Location.X + panel6.Width / 2, panel3.Location.Y + panel3.Height / 2,
                panel3.Location.X + panel3.Width , panel3.Location.Y + panel3.Height / 2);

            draw_arrow(panel1, mypen2, panel3.Location.X + panel3.Width, panel3.Location.Y + panel3.Height / 2, 1);

        }

        //用pen绘制两条连接线组成一个箭头，红色，width=2，i=0/1/2/3表示箭头方向：右左上下
        public void draw_arrow(Panel panelX, Pen penX, int x, int y, int i)
        {
            Point p = new Point(x, y);
            Point p1, p2;
            if (i == 0)//向右
            {
                p1 = new Point(p.X - 5, p.Y - 5);
                p2 = new Point(p.X - 5, p.Y + 5);
            }
            else if (i == 1)//向左
            {
                p1 = new Point(p.X + 5, p.Y - 5);
                p2 = new Point(p.X + 5, p.Y + 5);
            }
            else if (i == 2)//向上
            {
                p1 = new Point(p.X - 5, p.Y + 5);
                p2 = new Point(p.X + 5, p.Y + 5);
            }
            else//向下
            {
                p1 = new Point(p.X - 5, p.Y - 5);
                p2 = new Point(p.X + 5, p.Y - 5);
            }
            Pen mypen = penX;
            Panel thispanel = panelX;
            thispanel.CreateGraphics().DrawLine(mypen, p1, p);
            thispanel.CreateGraphics().DrawLine(mypen, p2, p);
        }

        //用pen绘制两条连接线组成一个箭头，红色，width=2，i=0/1/2/3表示箭头方向：右左上下
        public void draw_arrow2(PaintEventArgs e,Pen penX, int x, int y, int i)
        {
            Point p = new Point(x, y);
            Point p1, p2;
            if (i == 0)//向右
            {
                p1 = new Point(p.X - 5, p.Y - 5);
                p2 = new Point(p.X - 5, p.Y + 5);
            }
            else if (i == 1)//向左
            {
                p1 = new Point(p.X + 5, p.Y - 5);
                p2 = new Point(p.X + 5, p.Y + 5);
            }
            else if (i == 2)//向上
            {
                p1 = new Point(p.X - 5, p.Y + 5);
                p2 = new Point(p.X + 5, p.Y + 5);
            }
            else//向下
            {
                p1 = new Point(p.X - 5, p.Y - 5);
                p2 = new Point(p.X + 5, p.Y - 5);
            }
            Pen mypen = penX;
            e.Graphics.DrawLine(mypen, p1, p);
            e.Graphics.DrawLine(mypen, p2, p);
        }

        private void SelfTest_Load(object sender, EventArgs e)
        {

        }
    }
}

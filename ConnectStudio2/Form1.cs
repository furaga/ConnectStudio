using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectStudioLib;

namespace ConnectStudio2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void ArrowPict_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawLine(new Pen(Brushes.Black, 3)
                {
                    CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(8, 8)
                },
                new Point(0, ArrowPict.Height / 2), new Point(ArrowPict.Width, ArrowPict.Height / 2));
        }

        private void srcPic_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}

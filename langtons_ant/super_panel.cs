using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace langtons_ant
{
    internal class Super_Panel : System.Windows.Forms.Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
            //base.OnPaint(e);
            if (Program.main_image != null)
            { e.Graphics.DrawImage(Program.main_image, 0, 0); }
            //base.OnPaint(e);
        }
    }
}

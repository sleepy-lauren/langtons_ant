using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace langtons_ant
{
    public partial class Form1 : Form
    {

        int iteration = 0;
        public Form1()
        {
            InitializeComponent();
            

            //Need to wire the click events for all direction buttons
            foreach (Control c in groupBox1.Controls)
            {
                if (c.Name.StartsWith("lblD"))
                {
                    c.Click += clicked_direction;
                }
            }

            //Need to wire the click events for all color buttons
            foreach (Control c in groupBox1.Controls)
            {
                if (c.Name.StartsWith("lblC"))
                {
                    c.Click += clicked_color;
                }
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            if (Program.main_image != null)
            { e.Graphics.DrawImage(Program.main_image, 0, 0); }

        }

        private void clicked_direction(object sender, EventArgs e)
        {
            String current_text = ((Label)sender).Text;
            if (current_text == "R")
            {
                ((Label)sender).Text = "L";
                Program.step_directions[int.Parse(((Label)sender).Name.Replace("lblD", ""))] = 0;
            }
            else
            {
                ((Label)sender).Text = "R";
                Program.step_directions[int.Parse(((Label)sender).Name.Replace("lblD", ""))] = 1;
            }
            set_colors_and_directions();

        }

        private void clicked_color(object sender, EventArgs e)
        {
            Color current_color = ((Label)sender).BackColor;
            ColorDialog cd = new ColorDialog();
            cd.Color = current_color;
            cd.ShowDialog();
            ((Label)sender).BackColor = cd.Color;

            //Lets save the color for the label we clicked on to the color array in Program.cs
            set_colors_and_directions();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //MessageBox.Show(Color.Blue.ToArgb().ToString());
            Program.step_colors[0] = Color.Blue.ToArgb();
            set_colors_and_directions();
        }

        private void set_colors_and_directions()
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c.Name.StartsWith("lblC"))
                {
                    Program.step_colors[int.Parse(c.Name.Replace("lblC", ""))] = c.BackColor.ToArgb();

                }
                if (c.Name.StartsWith("lblD"))
                {
                    if (c.Text == "L")
                    {
                        Program.step_directions[int.Parse(c.Name.Replace("lblD", ""))] = 0;
                    }
                    else
                    {
                        Program.step_directions[int.Parse(c.Name.Replace("lblD", ""))] = 1;
                    }

                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Program.make_ant_movement(this);
            iteration += 1;
            this.lblIteration.Text = "Iteration:  " + iteration.ToString();
        }

        private void btnGoStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String s = "";
            for (int i = 0; i < 20; i++) {
                s += i.ToString() + ":  " + Color.FromArgb(Program.step_colors[i]).ToString() + "\n";
            }
            MessageBox.Show(s);
        }

        private void nudSteps_ValueChanged(object sender, EventArgs e)
        {
            Program.step_count = int.Parse(nudSteps.Value.ToString());
        }
    }
}

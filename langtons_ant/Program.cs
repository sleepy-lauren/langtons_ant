using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics.Eventing.Reader;

namespace langtons_ant
{
    internal static class Program
    {
        //Objects to dreaw and paint
        static Pen black_pen = new Pen(Color.Black);
        static Brush white_brush = new SolidBrush(Color.White);

        //Array to hold the steps -
        //Steps are basically like "if lands on a white square, turn right and paint the existing square black"
        //So there is a sequence of colors (white then black) and directions (left or right - 0 for L, 1 for R)
        //We can have up to 20 steps
        public static int[] step_colors = new int[20];
        public static int[] step_directions = new int[20];
        public static int step_count = 2;
        public static int current_step_number = 1;

        //Keep track of the location and the direction of the ant
        public static int[,] grid_colors = new int[100, 100];

        public enum  Direction:Int32
        {
            N = 0, E = 1 ,W= 2,S = 3
        }
        public static int current_x = 50;
        public static int current_y = 50;
        public static Direction current_direction = 0;

        //We will keep a picture of an ant in memory so that it can move around the grid.
        //It will be loaded one time, on the first painting.
        public static Bitmap ant_bitmap_north = null;
        public static Bitmap ant_bitmap_south = null;
        public static Bitmap ant_bitmap_east = null;
        public static Bitmap ant_bitmap_west = null;

        //Represents the image we will maintain including the background grid, the ant, and the colored cells
        public static Bitmap main_image = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //Initialize colors for the grid to the first possible color
            for(int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    grid_colors[i, j] = (step_colors[0]);
                }
            }
        }

        public static void make_ant_movement(Form calling_form)
        {
            int current_color;
            try
            {
                current_color = (grid_colors[current_x, current_y]);
            }
            catch (Exception e)
            {
                return;
            }

            
            int current_color_position = 0;
            for (int i = 0; i < 20; i++)
            {
                if (current_color == step_colors[i]) { current_color_position = i; break; }
            }

            //Now that we know the color that we are on, we turn a specific way, left or right
            int direction_to_turn = step_directions[current_color_position];

            //Flip the color of the current square to the next
            int next_color = current_color_position;
            next_color += 1;
            if (next_color == step_count) { next_color = 0; }
            grid_colors[current_x, current_y] = step_colors[next_color];
            

            //Figure out what direction we need to turn in
            switch (current_direction)
            {
                case Direction.N:  //0
                    current_x += ((direction_to_turn * 2) - 1);
                    //left goes west, right goes east
                    if (direction_to_turn == 0) {current_direction = Direction.W; }
                    if (direction_to_turn == 1) { current_direction = Direction.E; }
                    break;
                case Direction.S:  //3
                    current_x -= ((direction_to_turn * 2) - 1);
                    //left goes east, right goes west
                    if (direction_to_turn == 1) { current_direction = Direction.W; }
                    if (direction_to_turn == 0) { current_direction = Direction.E; }
                    break;
                case Direction.E: //1
                    current_y += ((direction_to_turn * 2) - 1);
                    //left goes north, right goes south
                    if (direction_to_turn == 0) { current_direction = Direction.N; }
                    if (direction_to_turn == 1) { current_direction = Direction.S; }
                    break;
                case Direction.W: //2
                    current_y -= ((direction_to_turn * 2) - 1);
                    //left goes south, right goes north
                    if (direction_to_turn == 1) { current_direction = Direction.N; }
                    if (direction_to_turn == 0) { current_direction = Direction.S; }
                    break;
                default: break;

            }

            //current_y += 1;
            main_image = draw_background_grid();
            calling_form.Refresh();
            current_step_number += 1;
            if (current_step_number > step_count) { current_step_number = 1; }
        }

        public static Bitmap draw_background_grid()
        {
            //Draw a grid - 1000 x 1000
            Bitmap background_image = new Bitmap(1000, 1000);
            Graphics g = Graphics.FromImage(background_image);

            g.FillRectangle(white_brush, 0, 0, 1000, 1000);
            for (int i = 0; i < 1000; i += 10)
            {
                for (int j = 0; j < 1000; j += 10)
                {
                    //vertical lines
                    g.DrawLine(black_pen, i, j, i, j + 10);
                    //horizontal lines
                    g.DrawLine(black_pen, i, j, i + 10, j);

                    //Fill the grid with colors that the ant created
                    g.FillRectangle(new SolidBrush(Color.FromArgb(grid_colors[i / 10, j / 10])), i, j, 10, 10 );
                }
            }

            //Draw the ant
            if ( ant_bitmap_north == null)
            {
                Assembly this_program = Assembly.GetExecutingAssembly();
                Stream this_pic = this_program.GetManifestResourceStream("langtons_ant.ant_north.bmp");
                ant_bitmap_north = new Bitmap(this_pic);

                this_pic = this_program.GetManifestResourceStream("langtons_ant.ant_south.bmp");
                ant_bitmap_south = new Bitmap(this_pic);

                this_pic = this_program.GetManifestResourceStream("langtons_ant.ant_east.bmp");
                ant_bitmap_east = new Bitmap(this_pic);

                this_pic = this_program.GetManifestResourceStream("langtons_ant.ant_west.bmp");
                ant_bitmap_west = new Bitmap(this_pic);
            }
            Bitmap ant_to_draw = null;
            switch (current_direction)
            {
                case Direction.N: ant_to_draw = ant_bitmap_north;
                    break;
                case Direction.S: ant_to_draw = ant_bitmap_south;
                    break;
                case Direction.E: ant_to_draw = ant_bitmap_east;
                    break;
                case Direction.W:  ant_to_draw = ant_bitmap_west;
                    break;
                default:
                    break;
                
            }

            g.DrawImage(ant_to_draw, (current_x * 10) + 10, (current_y * 10) + 10);



            //Draw the bitmap to the actual form
            //form_graphics.DrawImage(background_image, 10, 10);
            return background_image;
        }

    }
}

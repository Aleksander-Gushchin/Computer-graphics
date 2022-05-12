using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace wp
{

  public partial class Form1 : Form
  {
    //Bitmap bm = null;
    int x0, y0;

    Graphics g;
    Pen p, axis_pen;
    int height, width;
    Bitmap bm;
    public Form1()
    {
      InitializeComponent();

      height = pictureBox1.Height;
      width = pictureBox1.Width;
      bm = new Bitmap(width, height);
      g = Graphics.FromImage(bm);
      g.Clear(Color.White);
      p = new Pen(Color.Blue);
      axis_pen = new Pen(Color.Black);
      //axis_draw();
      x0 = 0;
      y0 = 0;

      pictureBox1.Image = bm;
    }

    //private Point get_point(int x, int y)
    //{
    //  return new Point(x + x0, height - (y + y0));
    //}

    //private Point get_point_rel(double x, double y)
    //{
    //  return new Point((int)(x*width), (int)((1 - y)*height));
    //}


    private void button1_Click(object sender, EventArgs e)
    {
      g.Clear(Color.White);


      draw_function();

      pictureBox1.Image = bm;
    }


    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
      axis_draw();


    }


    private void axis_draw()
    {

      pictureBox1.Image = bm;
    }


    //a*sin(b*x)
    private void draw_function()
    {
      RenderModule rd = new RenderModule(110, 100, height, width);

      rd.add(new Parallelepiped(-30, 10, 10, 40, 60, 100));

      //rd.add(new Parallelepiped(10, 0, 110, 20, 200, 30));

      rd.transform_all(Matrix.T(-100, -100, -100));
      rd.transform_all(  Matrix.R_y(Math.PI * Convert.ToDouble(textBox1.Text.Replace('.', ',')) * 1 / 4) * Matrix.R_x(Math.PI * 1 / 16));
      rd.transform_all(Matrix.T(100, 100, 100));
      rd.draw_all(g, p);
      pictureBox1.Image = bm;
    }
  }
}

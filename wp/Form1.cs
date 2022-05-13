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
      p = new Pen(Color.Black, 2);
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
      RenderModule rd = new RenderModule(width/2, height/2, height, width);


      int model_height = 480, model_width = 320, model_depth = 240;
      int base_h = 80, base_d = 240, base_w = 320;
      int x0 = model_width / 2, y0 = model_height / 2, z0 = model_depth / 2;

      //base
      rd.add(new Parallelepiped(0, -1, 80, base_h, base_w, 2*base_d/3));
      rd.add(new Parallelepiped(0, -1, 0, base_h, base_w/4, base_d/3 -1));
      rd.add(new Parallelepiped(3*base_w/4, -1, 0, base_h, base_w / 4, base_d / 3 -1));
      // column
      int col_wd = 20;
      rd.add(new Parallelepiped(10, base_h, 10, model_height - (base_h + 40), col_wd, col_wd));
      rd.add(new Parallelepiped(10, base_h, model_depth - (10 + col_wd), model_height - (base_h + 40), col_wd, col_wd));
      rd.add(new Parallelepiped(model_width - (10 + col_wd), base_h, 10, model_height - (base_h + 40), col_wd, col_wd));
      rd.add(new Parallelepiped(model_width - (10 + col_wd), base_h, model_depth - (10 + col_wd), model_height - (base_h + 40), col_wd, col_wd));
      // side 
      rd.add(new Parallelepiped(15, base_h, 10 + col_wd, model_height - (base_h + 40), 10, model_depth - 2 * (10 + col_wd)));
      rd.add(new Parallelepiped(model_width - 25, base_h, 10 + col_wd, model_height - (base_h + 40), 10, model_depth - 2 * (10 + col_wd)));
      // top
      rd.add(new Parallelepiped(0, model_height - (40 - 1), 0, 40, model_width, model_depth));
      //stairs
      rd.add(new Parallelepiped(base_w/4 + 1, -1, 0, base_h/3, base_w / 2 -2, 3 * base_d / 9));
      rd.add(new Parallelepiped(base_w/4 + 1, base_h / 3  - 1, base_d / 9, base_h/3, base_w / 2 -2, 2 * base_d / 9));
      rd.add(new Parallelepiped(base_w/4 + 1, 2 * base_h / 3  - 1, 2 * base_d / 9, base_h/3, base_w / 2 -2, base_d / 9));
      // column
      rd.add(new Cylinder(60, 0, -40 - 10, 300, 10));
      rd.add(new Cylinder(model_width - 60, 0, -40 - 10, 300, 10));

      //rd.add(new Parallelepiped(-30, 10, 10, 40, 60, 100));

      //rd.add(new Parallelepiped(10, 0, 110, 20, 200, 30));

      rd.transform_all(Matrix.T(-x0, -y0, -z0));
      rd.transform_all(  Matrix.R_y(Math.PI * Convert.ToDouble(textBox1.Text.Replace('.', ',')) * 1 / 4) * Matrix.R_x(- Math.PI * 1 / 4));

      rd.draw_all(g, p);
      pictureBox1.Image = bm;
    }
  }
}

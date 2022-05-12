using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;


namespace wp
{

  class Point3d
  {
    int[] xyz = new int[4];

    public int x
    {
      set => xyz[0] = value;
      get => xyz[0];
    }

    public int y
    {
      set => xyz[1] = value;
      get => xyz[1];
    }

    public int z
    {
      set => xyz[2] = value;
      get => xyz[2];
    }

    public Point3d(int[] arr)
    {
      if (arr.Length != 4)
        throw new ArgumentException("Wrong Arg");

      arr.CopyTo(xyz, 0);
    }

    public Point3d(int x, int y, int z)
    {
      xyz[0] = x;
      xyz[1] = y;
      xyz[2] = z;
      xyz[3] = 1;
    }

    public static bool operator==(Point3d a, Point3d b)
    {
      if (a is null || b is null)
        return (a is null && b is null);
      return (a.x == b.x && a.y == b.y && a.z == b.z);
    }

    public static bool operator !=(Point3d a, Point3d b)
    {
      if (a is null || b is null)
        return ((a is null) && !(b is null)) || (!(a is null) && (b is null));
      return  (a is null && b is null) && !(a.x == b.x && a.y == b.y && a.z == b.z);
    }

    public static double dist(Point3d first, Point3d second)
    {
      return Math.Sqrt(Math.Pow(first.x - second.x, 2) + Math.Pow(first.y - second.y, 2) + Math.Pow(first.z - second.z, 2));
    }

    public int this[int i]
    {
      get => xyz[i];
      set => xyz[i] = value;
    }


    public void transform(Matrix mat)
    {
      double[] res = { 0, 0, 0, 0 };
      for (int i = 0; i < 4; ++i)
      {
        for (int j = 0; j < 4; ++j)
          res[i] += mat[i, j] * xyz[j];
      }

      xyz = res.Select(i => (int)i).ToArray();
    }
    //static Point3d operator*()
  }

  class Matrix
  {
    double[,] data = new double[4, 4];
    public Matrix()
    {
      for (int i = 0; i < 16; i++)
        data[i / 4, i % 4] = 0;
    }

    public Matrix(double[] arr)
    {
      if (arr.Length != 16)
        throw new ArgumentException("Wrong Arg");


      for (int i = 0; i < 16; i++)
        data[i / 4, i % 4] = arr[i];
    }

    public double this[int i, int j]
    {
      get => data[i, j];
      set => data[i, j] = value;
    }

    public static Matrix R_x(double alpha)
    {
      double[] arr =
        { 1, 0, 0, 0,
          0, Math.Cos(alpha), Math.Sin(alpha), 0,
          0, -Math.Sin(alpha), Math.Cos(alpha), 0,
          0, 0, 0, 1 };

      return new Matrix(arr);
    }

    public static Matrix T(double a, double b, double c)
    {
      double[] arr =
        { 1, 0, 0, 0,
          0, 1, 0, 0,
          0, 0, 1, 0,
          a, b, c, 1 };

      return new Matrix(arr);
    }

    public static Matrix R_y(double alpha)
    {
      double[] arr =
        { Math.Cos(alpha), 0, -Math.Sin(alpha), 0,
          0, 1, 0, 0,
          Math.Sin(alpha), 0, Math.Cos(alpha), 0,
          0, 0, 0, 1 };

      return new Matrix(arr);
    }

    public static Matrix operator *(Matrix mat1, Matrix mat2)
    {
      Matrix res = new Matrix();
      for (int i = 0; i < 4; ++i)
        for (int j = 0; j < 4; ++j)
          for (int k = 0; k < 4; ++k)
            res[i, j] += mat1[i, k] * mat2[k, j];

      return res;
    }

    public static Point3d operator *(Matrix mat, Point3d point)
    {
      double[] res = { 0, 0, 0, 0 };
      for (int i = 0; i < 4; ++i)
      {
        for (int j = 0; j < 4; ++j)
          res[i] += mat[i, j] * point[j];
      }

      return new Point3d(res.Select(i => (int)i).ToArray());
    }
  }


  class Line
  {
    Point3d first, second;

    public Point3d Pa
    {
      get => first;
    }

    public Point3d Pb
    {
      get => second;
    }

    public Line(int x0, int y0, int z0, int x1, int y1, int z1)
    {
      first  = new Point3d(x0, y0, z0);
      second = new Point3d(x1, y1, z1);
    }

    public static bool operator==(Line a, Line b)
    {
      return (a is null && b is null) || (a.first == b.first && a.second == b.second) || a.second == b.first && a.first == b.second; 
    }

    public static bool operator !=(Line a, Line b)
    {
      if (a is null || b is null)
        return ((a is null) && !(b is null)) || (!(a is null) && (b is null));
      return !(a.first == b.first && a.second == b.second);
    }


    


    private int det(int a, int b, int c, int d)
    {
      return a * d - b * c;
    }

    private bool between(int a, int b, double c)
    {
      const double EPS = 1E-9;
      return Math.Min(a, b) <= c + EPS && c <= Math.Max(a, b) + EPS;
    }

    private bool intersect_1(int a, int b, int c, int d)
    {
      if (a > b) (a, b) = (b, a);
      if (c > d) (c, d) = (d, c);
      return Math.Max(a, c) <= Math.Min(b, d);
    }

    public (bool, Point3d) intersect(Line line)
    {

      int A1 = first.y - second.y, B1 = second.x - first.x, C1 = -A1 * first.x - B1 * first.y;
      int A2 = line.first.y - line.second.y, B2 = line.second.x - line.first.x, C2 = -A2 * line.first.x - B2 * line.first.y;
      int zn = det(A1, B1, A2, B2);
      if (zn != 0)
      {
        double x = (double)(-det(C1, B1, C2, B2)) / zn;
        double y = (double)(-det(A1, C1, A2, C2)) / zn;


        bool res = between(first.x, second.x, x) && between(first.y, second.y, y)
          && between(line.first.x, line.second.x, x) && between(line.first.y, line.second.y, y);

        return (res, new Point3d((int)x, (int)y, int.MinValue));
      }
      else
        return (det(A1, C1, A2, C2) == 0 && det(B1, C1, B2, C2) == 0
          && intersect_1(first.x, second.x, line.first.x, line.second.x)
          && intersect_1(first.y, second.y, line.first.y, line.second.y), null);
    }

    public int find_z(int x, int y)
    {
      double t = 0;

      if (second.x - first.x != 0 && Math.Abs(second.x - first.x) > Math.Abs(second.y - first.y))
        t = (double)(x - first.x) / (second.x - first.x);
      if (second.y - first.y != 0 && Math.Abs(second.x - first.x) <= Math.Abs(second.y - first.y))
        t = (double)(y - first.y) / (second.y - first.y);

      return (int)(first.z * (1 - t) + second.z * t);
    }

    public bool on_line(int x, int y)
    {
      //(mx - ax) * (by - ay) == (bx - ax) * (my - ay)
      return ((x - first.x) * (second.x - first.x) == (second.x - first.x) * (y - first.y)) 
        && ((Math.Min(first.x, second.x) <= x && Math.Max(first.x, second.x) >= x) && (Math.Min(first.y, second.y) <= y && Math.Max(first.y, second.y) >= y));
    }

    public Line(Point3d _first, Point3d _second)
    {
      first = _first;
      second = _second;
    }
  }

  abstract class Poly
  {
    protected List<Point3d> points;
    protected List<Line> lines;

    public abstract List<Line> find_lines(Line line);
    public bool point_find(Point3d point)
    {
      foreach (var line in lines)
      {
        if (line.on_line(point.x, point.y))
          return true;
      }

      //
      bool result = false;
      int j = points.Count() - 1;
      int size = points.Count();
      for (int i = 0; i < size; i++)
      {
        if ((points[i].y < point.y && points[j].y >= point.y || points[j].y < point.y && points[i].y >= point.y) &&
             (points[i].x + (double)(point.y - points[i].y) / (points[j].y - points[i].y) * (points[j].x - points[i].x) < point.x))
          result = !result;
        j = i;
      }
      //

      //Point3d p = new Point3d(int.MaxValue, point.y);
      //Line l =  new Line(_first)
      //foreach(var line in lines)
      //{
      //  (isIntersec, point) = line()
      //}
      return result;
    }

    private (double, double, double) get_coef()
    {
      double[,] mat  = new double[3, 3];
      Func<double[,], double> det = m => 
        + m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) 
        - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) 
        + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);


      Point3d first, second, third;

      first = points[0];
      second = points[1];
      third = points[2];

      (mat[0, 0], mat[0, 1], mat[0, 2]) = (first.x, first.y, first.z);
      (mat[1, 0], mat[1, 1], mat[1, 2]) = (second.x, second.y, second.z);
      (mat[2, 0], mat[2, 1], mat[2, 2]) = (third.x, third.y, third.z);

      
      double[,] matA = mat.Clone() as double[,];
      double[,] matB = mat.Clone() as double[,];
      double[,] matC = mat.Clone() as double[,];


      (matA[0, 0], matA[1, 0], matA[2, 0]) = (-1, -1, -1);
      (matB[0, 1], matB[1, 1], matB[2, 1]) = (-1, -1, -1);
      (matC[0, 2], matC[1, 2], matC[2, 2]) = (-1, -1, -1);

      double detG = det(mat);

      return (det(matA) / detG, det(matB) / detG, det(matC) / detG);
    }

    public (bool, int) find_z(int x, int y)
    {
      var (A, B, C) = get_coef();
      if (Math.Abs(C) < 1e-5)
        return (false, 0);
      return (true, (int)((-1 - A * x - B * y) / C));
    }

  }

  class Face : Poly
  {
    
    //clockwise
    public Face(Point3d vertex1, Point3d vertex2, Point3d vertex3, Point3d vertex4)
    {
      points = new List<Point3d> { vertex1, vertex2, vertex3, vertex4 };
      lines = new List<Line> { new Line(vertex1, vertex2), new Line(vertex2, vertex3), new Line(vertex3, vertex4), new Line(vertex4, vertex1) };
    }


    public override List<Line> find_lines(Line line)
    {
      List<Line> res = new List<Line>();
      //res.Add(line);
      // if they laying in 

      foreach(var l in lines)
      {
        if(l == line)
        {
          res.Add(line);
          return res;
        }
      }

      bool added = false;

      int num = 0;
      if (point_find(line.Pa))
        num++;
      if (point_find(line.Pb))
        num++;


      if (num == 2)
      {
        added = true;
        var (nonNullC, z1) = find_z(line.Pa.x, line.Pa.y);
        var (_, z2) = find_z(line.Pb.x, line.Pb.y);
        if (!nonNullC || (z1 >= line.Pa.z && z2 >= line.Pb.z))
        {
          res.Add(line);
          return res;
        }
      }

      if(num == 0)
      {
        int n = 0;
        added = true;
        List<Point3d> inter_points = new List<Point3d>(); 
        foreach (var l in lines)
        {
          var (isIntersec, point) = l.intersect(line);
          if (isIntersec && point == null)
          {
            res.Add(line);
            return res;
          }

          if(isIntersec && point != null)
          {
            inter_points.Add(point);
            n++;
          }
        }

        if (n >= 2)
        {
          
          var (nonNullC, z1) = find_z(inter_points[0].x, inter_points[0].y);
          var (_, z2) = find_z(inter_points[1].x, inter_points[1].y);

          int z_line1 = line.find_z(inter_points[0].x, inter_points[0].y);
          int z_line2 = line.find_z(inter_points[1].x, inter_points[1].y);

          inter_points[0].z = z_line1;
          inter_points[1].z = z_line2;

          if (z_line1 < z1 && z_line2 < z2)
          {
            res.Add(line);
            return res;
          }
          else
          {
            if(Point3d.dist(line.Pa, inter_points[0]) < Point3d.dist(line.Pa, inter_points[1]))
            {
              if (Point3d.dist(line.Pa, inter_points[0]) > 1e-5)
                res.Add(new Line(line.Pa, inter_points[0]));
              if (Point3d.dist(line.Pb, inter_points[1]) > 1e-5)
                res.Add(new Line(line.Pb, inter_points[1]));
            }
            else
            {
              if (Point3d.dist(line.Pa, inter_points[1]) > 1e-5)
                res.Add(new Line(line.Pa, inter_points[1]));
              if (Point3d.dist(line.Pb, inter_points[0]) > 1e-5)
                res.Add(new Line(line.Pb, inter_points[0]));
            }
          }
        }
        else
        {
          res.Add(line);
          return res;
        }
      }

      if (num == 1)
      {
        added = true;
        Point3d outs_point = !point_find(line.Pa) ? line.Pa : line.Pb;
        Point3d in_point = point_find(line.Pa) ? line.Pa : line.Pb;
        if (in_point == outs_point)
          throw new Exception("Same points");

        bool isIntersec = false;
        Point3d point =  null, true_point = null;
        foreach (var l in lines)
        {
          Point3d local_point;
          (isIntersec, local_point) = l.intersect(line);
          if(isIntersec && local_point != null)
          {
            point = local_point;
            if (!(point.x == in_point.x && point.y == in_point.y))
              true_point = point;
          }

          
        }

        if (point == null || true_point == null)
        {

          res.Add(line);
          return res;
        }

        if (true_point != null)
        {
          point = true_point;
          var (NonNullC, z) = find_z(point.x, point.y);

          int z_line = line.find_z(point.x, point.y);
          point.z = z_line;

          if (z_line <= z)
          {
            res.Add(line);
            return res;
          }
          else
          {
            if (Point3d.dist(outs_point, point) > 1e-5)
              res.Add(new Line(outs_point, point));
            return res;
          }
        }
        //throw new Exception("a");
      }

      if (!added)
      {
        throw new Exception("B");
        //res.Add(line);
      }
      return res;
    }
    //public int find_z_val(int x, int y)
    //{

    //}
  }

  abstract class Object3d
  {
    protected Poly[] faces;
    protected List<Point3d> points;
    protected List<Line> lines;
    public void transform(Matrix mat)
    {
      points.ForEach(p => p.transform(mat));
    }

    public List<Line> get_lines(List<Object3d> list_of_obj)
    {
      List<Line> res = new List<Line>(lines);

      foreach (var obj in list_of_obj)
      {
        for (int i = 0; i < res.Count; ++i)
        {
          Line line = res[i];
          List<Line> inter_res = obj.intersection(line);
          res.RemoveAt(i);
          res.InsertRange(i, inter_res);
          if (inter_res.Count == 0)
          {
            --i;
          }
          if (inter_res.Count > 2)
            throw new Exception("too many lines");

          if (res.Count == 0)
            return res;
        }
      }



      return res;
    }


    public List<Line> intersection(Line line)
    {
      List<Line> res = new List<Line>();

      res.Add(line);

      foreach (var f in faces)
      {

        for (int i = 0; i < res.Count; ++i)
        {
          var line_list = f.find_lines(res[i]);
          res.RemoveAt(i);
          res.InsertRange(i, line_list);
          if (line_list.Count == 0)
          {
            --i;
            //break;
          }
          if (res.Count == 0)
            return res;
        }
      }

      return res;
    }

  }

  class Parallelepiped : Object3d
  {
    //Face[] faces;
    public Parallelepiped(int x0, int y0, int z0, int h, int w, int d)
    {
     

      List<Point3d> points_list = new List<Point3d> { new Point3d(x0, y0, z0), new Point3d(x0, y0 + h, z0), new Point3d(x0 + w, y0 + h, z0), new Point3d(x0 + w, y0, z0),
        new Point3d(x0, y0, z0 + d), new Point3d(x0, y0 + h, z0 + d), new Point3d(x0 + w, y0 + h, z0 + d), new Point3d(x0 + w, y0, z0 + d) };


      faces = new Face[6];
      lines = new List<Line>(new Line[12]);
      points = points_list;

      
      for(int i = 0; i < 4; ++i)
      {
        lines[3 * i] = new Line(points[i], points[(i + 1) % 4]);
        lines[3 * i + 1] = new Line(points[i], points[i + 4]);
        lines[3 * i + 2] = new Line(points[i + 4], points[(i + 1) % 4 + 4]);

      }

      faces[0] = new Face(points_list[0], points_list[1], points_list[2], points_list[3]);
      faces[1] = new Face(points_list[4], points_list[5], points_list[6], points_list[7]);
      faces[2] = new Face(points_list[0], points_list[1], points_list[5], points_list[4]);
      faces[3] = new Face(points_list[2], points_list[6], points_list[7], points_list[3]);
      faces[4] = new Face(points_list[1], points_list[5], points_list[6], points_list[2]);
      faces[5] = new Face(points_list[0], points_list[4], points_list[7], points_list[3]);
    }




  }

  class Cylinder : Object3d
  {
    Cylinder(int x, int y, int z, int h, double r)
    {

    }


    new public void transform(Matrix mat)
    {
      points.ForEach(p => p.transform(mat));
      //Todo
    }
  }

  class RenderModule
  {
    //List<Point3d> CoordList;


    int x0, y0;
    int height, width;
    List<Object3d> all_object;

    public RenderModule(int _x0, int _y0, int _height, int _width)
    {
      all_object = new List<Object3d>();
      x0 = _x0;
      y0 = _y0;
      height = _height;
      width = _width;
    }

    public void add(Object3d obj)
    {
      all_object.Add(obj);
    }

    public void transform_all(Matrix mat)
    {
      all_object.ForEach(p => p.transform(mat));
    }

    public void draw_all(Graphics g, Pen p)
    {
      List<Line> line_list = new List<Line>();
      all_object.ForEach(obj => line_list.AddRange(obj.get_lines(all_object)));

      //line_list.RemoveAt(1);
      line_list.ForEach(line => g.DrawLine(p, get_point(line.Pa), get_point(line.Pb)));
    }

    public Point get_point(Point3d point)
    {
      int res_x = x0 + point.x;
      int res_y = height - (y0 + point.y);
      return new Point(res_x, res_y);
    }

  }
}

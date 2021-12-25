using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Cornish_Room
{
    public class Point3
    {
        public double x;
        public double y;
        public double z;

        public Point3() { x = 0; y = 0; z = 0; }

        public Point3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3(Point3 p)
        {
            if (p == null)
                return;
            x = p.x;
            y = p.y;
            z = p.z;
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static Point3 Normalize(Point3 p)
        {
            double z = Math.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
            return z == 0 ? new Point3(p) : new Point3(p.x / z, p.y / z, p.z / z);
        }

        public static double Scalar_Multiple(Point3 p1, Point3 p2)
        {
            return p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
        }

        public static Point3 operator -(Point3 p1, Point3 p2)
        {
            return new Point3(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);

        }

        public static Point3 operator +(Point3 p1, Point3 p2)
        {
            return new Point3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);

        }

        public static Point3 operator *(Point3 p1, Point3 p2)
        {
            return new Point3(p1.y * p2.z - p1.z * p2.y, p1.z * p2.x - p1.x * p2.z, p1.x * p2.y - p1.y * p2.x);
        }

        public static Point3 operator *(double t, Point3 p1)
        {
            return new Point3(p1.x * t, p1.y * t, p1.z * t);
        }


        public static Point3 operator *(Point3 p1, double t)
        {
            return new Point3(p1.x * t, p1.y * t, p1.z * t);
        }

        public static Point3 operator -(Point3 p1, double t)
        {
            return new Point3(p1.x - t, p1.y - t, p1.z - t);
        }

        public static Point3 operator -(double t, Point3 p1)
        {
            return new Point3(t - p1.x, t - p1.y, t - p1.z);
        }

        public static Point3 operator +(Point3 p1, double t)
        {
            return new Point3(p1.x + t, p1.y + t, p1.z + t);
        }

        public static Point3 operator +(double t, Point3 p1)
        {
            return new Point3(p1.x + t, p1.y + t, p1.z + t);
        }

        public static Point3 operator /(Point3 p1, double t)
        {
            return new Point3(p1.x / t, p1.y / t, p1.z / t);
        }

        public static Point3 operator /(double t, Point3 p1)
        {
            return new Point3(t / p1.x, t / p1.y, t / p1.z);
        }
    }

    public class Edge
    {
        public List<Point3> points;
        public double[] material_values = new double[5];
        public Color color = Color.Black;
        public Edge()
        {
            this.points = new List<Point3> { };
            this.color = Color.Black;
        }
        public Edge(List<Point3> p)
        {
            this.points = p;
        }
    }

    public class Polyhedron
    {
        public List<Edge> edges;
        public double[] Material = new double[5];
        public Color MaterialColor = new Color();
        public Polyhedron()
        {
            this.edges = new List<Edge> { };
        }
        public Polyhedron(List<Edge> e)
        {
            this.edges = e;
        }
        public void Set_material(double[] material)
        {
            foreach (var x in edges)
                x.material_values = material;
        }
        public bool IntersectRayWithTriangle(RayTrace RayTrace, Point3 p0, Point3 p1, Point3 p2, out double intersect)
        {
            var eps = 0.0001;
            Point3 edge1 = p1 - p0;
            Point3 edge2 = p2 - p0;
            Point3 h = RayTrace.direction * edge2;
            double a = Point3.Scalar_Multiple(edge1, h);
            intersect = -1;
            if (a > -eps && a < eps)
                return false;

            Point3 s = RayTrace.first_p - p0;
            double u = Point3.Scalar_Multiple(s, h) / a;
            if (u < 0 || u > 1)
                return false;

            Point3 q = s * edge1;
            double v = Point3.Scalar_Multiple(RayTrace.direction, q) / a;
            if (v < 0 || u + v > 1)
                return false;

            double t = Point3.Scalar_Multiple(edge2, q) / a;
            if (t < eps)
                return false;

            intersect = t;
            return true;
        }
        public virtual bool IntersectFigures(RayTrace r, out double intersect, out Point3 normal)
        {
            intersect = 0;
            normal = null;
            Edge side = null;
            foreach (var figure_side in edges)
            {
                if (figure_side.points.Count == 3)
                {
                    if (IntersectRayWithTriangle(r, figure_side.points[0], figure_side.points[1], figure_side.points[2], out double t) && (intersect == 0 || t < intersect))
                    {
                        intersect = t;
                        side = figure_side;
                    }
                }
                else if (figure_side.points.Count == 4)
                {
                    if (IntersectRayWithTriangle(r, figure_side.points[0], figure_side.points[1], figure_side.points[3], out double t) && (intersect == 0 || t < intersect))
                    {
                        intersect = t;
                        side = figure_side;
                    }
                    else if (IntersectRayWithTriangle(r, figure_side.points[1], figure_side.points[2],  figure_side.points[3], out t) && (intersect == 0 || t < intersect))
                    {
                        intersect = t;
                        side = figure_side;
                    }
                }
            }
            if (intersect != 0)
            {
                normal = Point3.Normalize((side.points[1] - side.points[0]) * (side.points[side.points.Count - 1] - side.points[0]));
                MaterialColor = side.color;
                Material = side.material_values;
                return true;
            }
            return false;
        }

        public static Polyhedron Hex(int size)
        {
            var hc = size / 2;
            Polyhedron p = new Polyhedron();
            Edge e = new Edge();
            e.points = new List<Point3> {
                new Point3(-hc, hc, -hc), 
                new Point3(hc, hc, -hc), 
                new Point3(hc, -hc, -hc), 
                new Point3(-hc, -hc, -hc)
            };
            p.edges.Add(e);
            e = new Edge();

            e.points = new List<Point3> {
                new Point3(-hc, hc, -hc), 
                new Point3(-hc, hc, hc), 
                new Point3(hc, hc, hc), 
                new Point3(hc, hc, -hc) 
            };
            p.edges.Add(e);
            e = new Edge();

            
            e.points = new List<Point3> {
                
                new Point3(-hc, hc, hc),
                new Point3(-hc, -hc, hc), 
                new Point3(hc, -hc, hc), 
                new Point3(hc, hc, hc)
            };
            p.edges.Add(e);
            e = new Edge();

            e.points = new List<Point3> {
                new Point3(hc, hc, hc), 
                new Point3(hc, -hc, hc),
                new Point3(hc, -hc, -hc),
                new Point3(hc, hc, -hc) 
            };
            p.edges.Add(e);
            e = new Edge();

           
            e.points = new List<Point3> {
                new Point3(-hc, hc, hc), 
                new Point3(-hc, hc, -hc), 
                new Point3(-hc, -hc, -hc), 
                new Point3(-hc, -hc, hc) 
            };
            p.edges.Add(e);
            e = new Edge();

            
            e.points = new List<Point3> {
                new Point3(-hc, -hc, -hc),
                new Point3(hc, -hc, -hc),
                new Point3(hc, -hc, hc),
                new Point3(-hc, -hc, hc)
            };
            p.edges.Add(e);
            e = new Edge();

            return p;

        }
    }

    public class Sphere : Polyhedron
    {
        double radius;
        static double eps = 0.0001;
        public Sphere(Point3 p, double r)
        {
            this.edges.Add(new Edge(new List<Point3>()));
            this.edges[0].points.Add(p);
            radius = r;
        }

        public override bool IntersectFigures(RayTrace r, out double t, out Point3 normal)
        {
            normal = null;
            Point3 sphere_pos = edges[0].points[0];
            Point3 k = r.first_p - sphere_pos;
            double b = Point3.Scalar_Multiple(k, r.direction);
            double c = Point3.Scalar_Multiple(k, k) - radius * radius;
            double d = b * b - c;
            t = 0;
            if (d >= 0)
            {
                double sqrtd = Math.Sqrt(d);
                double t1 = -b + sqrtd;
                double t2 = -b - sqrtd;

                double min_t = Math.Min(t1, t2);
                double max_t = Math.Max(t1, t2);

                t = (min_t > eps) ? min_t : max_t;
            }
            if (t > eps)
            {
                normal = (r.first_p + r.direction * t) - edges[0].points[0];
                normal = Point3.Normalize(normal);

                MaterialColor = edges[0].color;
                Material = edges[0].material_values;
                return true;
            }
            return false;
        }
    }

    public class RayTrace
    {
        public Point3 first_p;
        public Point3 direction;

        public RayTrace(Point3 st, Point3 end)
        {
            first_p = new Point3(st);
            direction = Point3.Normalize(end - st);
        }

        public RayTrace() { }

        public RayTrace(RayTrace r)
        {
            first_p = r.first_p;
            direction = r.direction;
        }
        public RayTrace Refract(Point3 point_light_inter, Point3 normal, double refraction, double k_refract)
        {
            RayTrace new_ray = new RayTrace();
            new_ray.first_p = new Point3(point_light_inter);

            double Scalar_Multiple = normal.x * direction.x + normal.y * direction.y + normal.z * direction.z;
            double refract_result = refraction / k_refract;
            double theta_formula = 1 - refract_result * refract_result * (1 - Scalar_Multiple * Scalar_Multiple);
            if (theta_formula >= 0)
            {
                double cos_theta = Math.Sqrt(theta_formula);
                new_ray.direction = Point3.Normalize(direction * refract_result - (cos_theta + refract_result * Scalar_Multiple) * normal);
                return new_ray;
            }
            else
                return null;
        }
        public RayTrace Reflect(Point3 point_light_inter, Point3 normal)
        {
            Point3 reflect_dir = direction - 2 * normal * Point3.Scalar_Multiple(direction, normal);
            return new RayTrace(point_light_inter, point_light_inter + reflect_dir);
        }
    }

    public class Light
    {
        public Point3 first_p;

        public Light(Point3 p)
        {
            first_p = new Point3(p);
        }

        public Point3 Shading(Point3 point_light_inter, Point3 normal, Color MaterialColor, double diffuse_value)
        {
            Point3 dir = first_p - point_light_inter;
            dir = Point3.Normalize(dir);
            Point3 diff;
            diff = diffuse_value * new Point3(1f, 1f, 1f) * Math.Max(Point3.Scalar_Multiple(normal, dir), 0.1);
            return new Point3(diff.x * MaterialColor.R / 255.0, diff.y * MaterialColor.G / 255.0, diff.z * MaterialColor.B / 255.0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cornish_Room
{
    public partial class Form1 : Form
    {
        static Bitmap bmp;
        List<Polyhedron> FiguresList = new List<Polyhedron>();
        public int height, width;
        public Color[,] colors;
        public Point3 camPos = new Point3();
        public List<Light> lights = new List<Light>();
        public Point3[,] pixels;
        public Form1()
        {
            InitializeComponent();
            height = pictureBox1.Height;
            width = pictureBox1.Width;
            bmp = new Bitmap(width, height);
            colors = new Color[width, height];
            pixels = new Point3[width, height];
            Ray_Tracing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bmp = new Bitmap(width, height);
            colors = new Color[width, height];
            pixels = new Point3[width, height];
            camPos = new Point3();
            lights = new List<Light>();
            FiguresList = new List<Polyhedron>();
            Ray_Tracing();
        }

        

        public void Figures_Create()
        {
            
            Polyhedron CubeRoom = Polyhedron.Hex(12);
            var edge_number = 5;
            var back_edge = CubeRoom.edges[edge_number];
            Point3 normal = Point3.Normalize((back_edge.points[1] - back_edge.points[0]) * (back_edge.points[back_edge.points.Count - 1] - back_edge.points[0]));

            Point3 center = new Point3();
            foreach (var x in CubeRoom.edges[edge_number].points)
                center += x;

            camPos =  center / 4 + normal * 15;

            CubeRoom.edges[0].color = Color.Green;
            CubeRoom.edges[1].color = Color.Yellow;
            CubeRoom.edges[2].color = Color.Blue;
            CubeRoom.edges[3].color = Color.BurlyWood;
            CubeRoom.edges[4].color = Color.Crimson;
            CubeRoom.edges[5].color = Color.Blue;
            foreach(var x in CubeRoom.edges)
            {
                x.material_values = new double[] { 0, 0, 0.05, 0.7, 1 };
            }
            if (checkBox7.Checked) CubeRoom.edges[1].material_values[0] = 1;
            if (checkBox8.Checked) CubeRoom.edges[4].material_values[0] = 1;
            if (checkBox9.Checked) CubeRoom.edges[3].material_values[0] = 1;
            if (checkBox10.Checked) CubeRoom.edges[5].material_values[0] = 1;

            FiguresList.Add(CubeRoom);

            Light l1 = new Light(new Point3(-3f, -4f, 1.9f));
            Light l2 = new Light(new Point3(double.Parse(textBox1.Text), double.Parse(textBox2.Text) , double.Parse(textBox3.Text)));
            if (checkBox5.Checked)
                lights.Add(l1);
            if (checkBox6.Checked)
                lights.Add(l2);

            Polyhedron CubeWithReflection = Polyhedron.Hex(4);
            CubeWithReflection.edges = Aphine_transforms.Rotate(CubeWithReflection, 30, 10, 10).edges;
            CubeWithReflection.edges = Aphine_transforms.Move(CubeWithReflection, -3, 0, -2).edges;
            if (checkBox1.Checked)
                CubeWithReflection.Set_material(new double[] { 1, 0, 0.1, 0.7, 1.5 });
            else
                CubeWithReflection.Set_material(new double[] { 0, 0, 0.1, 0.7, 1.5 });
            foreach (var x in CubeWithReflection.edges)
                x.color = Color.White;
            FiguresList.Add(CubeWithReflection);


            Polyhedron CubeWithRefraction = Polyhedron.Hex(5);
            CubeWithRefraction.edges = Aphine_transforms.Rotate(CubeWithRefraction, 10, 0, 0).edges;
            CubeWithRefraction.edges = Aphine_transforms.Move(CubeWithRefraction, 3, 0, 2).edges;
            if (checkBox3.Checked)
                CubeWithRefraction.Set_material(new double[] { 0, 0.7, 0.1, 0.5, 1.05 });
            else
                CubeWithRefraction.Set_material(new double[] { 0, 0, 0.1, 0.7, 1.5 });
            foreach (var x in CubeWithRefraction.edges)
                x.color = Color.Red;
            FiguresList.Add(CubeWithRefraction);


            Sphere BallWithReflection = new Sphere(new Point3(2.5, 2, 0), 2.5);
            BallWithReflection.edges = Aphine_transforms.Move(BallWithReflection, 0, 0, -2).edges;
            if (checkBox2.Checked)
                BallWithReflection.Set_material(new double[] { 1, 0, 0, 0.1, 1 });
            else
                BallWithReflection.Set_material(new double[] { 0, 0, 0.1, 0.4, 1.5 });
            foreach (var x in BallWithReflection.edges)
                x.color = Color.Blue;
            BallWithReflection.MaterialColor = Color.Blue;
            FiguresList.Add(BallWithReflection);


            Sphere BallWithRefraction = new Sphere(new Point3(2.5f, 2, 0f), 1.5f);
            BallWithRefraction.edges = Aphine_transforms.Move(BallWithRefraction, -4, 4, 1).edges;
            if (checkBox4.Checked)
                BallWithRefraction.Set_material(new double[] { 0f, 0.9f, 0.1f, 0.5f, 1.05f });
            else
                BallWithRefraction.Set_material(new double[] { 0, 0, 0.1, 0.1, 1 });
            foreach (var x in BallWithRefraction.edges)
                x.color = Color.Violet;
            BallWithRefraction.MaterialColor = Color.Green;
            FiguresList.Add(BallWithRefraction);
        }

        public void Pixels_Create()
        {

            var points = FiguresList[0].edges[5].points;

            Point3 up_shift = (points[1] - points[0]) / (width - 1);
            Point3 down_shift = (points[2] - points[3]) / (width - 1);
            Point3 up = new Point3(points[0]);
            Point3 down = new Point3(points[3]);


            for (int i = 0; i < width; ++i)
            {
                Point3 step_y = (up - down) / (height - 1);
                Point3 d = new Point3(down);
                for (int j = 0; j < height; ++j)
                {
                    pixels[i, j] = d;
                    d += step_y;
                }
                up += up_shift;
                down += down_shift;
            }
        }

        public Point3 RayTrace(RayTrace r, int iter, double env)
        {
            if (iter <= 0)
                return new Point3(0, 0, 0);
            double shortest_intersect = 0;
            Point3 normal = null;
            double[] Material = new double[5];
            Color MaterialColor = new Color();
            Point3 res_color = new Point3(0, 0, 0);
            bool is_sharp = false;

            foreach (var figure in FiguresList)
            {
                if (figure.IntersectFigures(r, out double intersect, out Point3 Normalize))
                    if (intersect < shortest_intersect || shortest_intersect == 0)
                    {
                        shortest_intersect = intersect;
                        normal = Normalize;
                        Material = figure.Material;
                        MaterialColor = figure.MaterialColor;
                    }
            }

            if (shortest_intersect == 0)
                return new Point3(0, 0, 0);

            if (Point3.Scalar_Multiple(r.direction, normal) > 0)
            {
                normal *= -1;
                is_sharp = true;
            }

            Point3 point_light_inter = r.first_p + r.direction * shortest_intersect;

            foreach (Light light in lights)
            {
                Point3 ambient_coef = new Point3(1f, 1f, 1f) * Material[2];
                ambient_coef.x = (ambient_coef.x * MaterialColor.R/255.0);
                ambient_coef.y = (ambient_coef.y * MaterialColor.G/255.0);
                ambient_coef.z = (ambient_coef.z * MaterialColor.B/255.0);
                res_color += ambient_coef;
                if (VisiblePoint(light.first_p, point_light_inter))
                    res_color += light.Shading(point_light_inter, normal, MaterialColor, Material[3]);
                else
                    res_color += light.Shading(point_light_inter, normal, MaterialColor, Material[3]) / 2;
            }

            if (Material[0] > 0)
            {
                RayTrace reflected_ray = r.Reflect(point_light_inter, normal);
                res_color = Material[0] * RayTrace(reflected_ray, iter - 1, env);
            }


            if (Material[1] > 0)
            {
                double refract_value;
                if (is_sharp)
                    refract_value = Material[4];
                else
                    refract_value = 1 / Material[4];

                RayTrace refracted_ray = r.Refract(point_light_inter, normal, Material[1], refract_value);

                if (refracted_ray != null)
                    res_color = Material[1] * RayTrace(refracted_ray, iter - 1, Material[4]);
            }
            return res_color;
        }
        public bool VisiblePoint(Point3 light_point, Point3 point_light_inter)
        {
            double max_t = (light_point - point_light_inter).Length();
            RayTrace r = new RayTrace(point_light_inter, light_point);
            foreach (var figure in FiguresList)
                if (figure.IntersectFigures(r, out double t, out Point3 n))
                    if (t < max_t && t > 0.0001)
                        return false;
            return true;
        }

        public void Ray_Tracing()
        {
            Figures_Create();

            Pixels_Create();

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                {
                    RayTrace r = new RayTrace(camPos, pixels[i, j]);
                    r.first_p = new Point3(pixels[i, j]);
                    Point3 color = RayTrace(r, 10, 1);//луч,кол-во итераций,коэфф
                    if (color.x > 1.0f || color.y > 1.0f || color.z > 1.0f)
                        color = Point3.Normalize(color);
                    colors[i, j] = Color.FromArgb((int)(255 * color.x), (int)(255 * color.y), (int)(255 * color.z));
                }

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    bmp.SetPixel(i, j, colors[i, j]);
            pictureBox1.Image = bmp;
        }
    }
}

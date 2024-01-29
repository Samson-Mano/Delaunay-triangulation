using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Delaunay_triangulation
{
    public partial class Form1 : Form
    {
        planar_object_store main_drw_obj;
        Random rand0 = new Random();

        public class planar_object_store // This is a sattelite class to store and control all the drawing ojects
        {
            List<point2d> _all_points = new List<point2d>(); // List of point object to store all the points in the drawing area
            List<edge2d> _all_edges = new List<edge2d>(); // List of edge object to store all the edges created from Delaunay triangulation
            List<face2d> _all_faces = new List<face2d>(); // List of face object to store all the faces created from Delaunay triangulation
            List<instance_tracker> _all_instance = new List<instance_tracker>(); // instance tracker to animate the results

            public List<instance_tracker> all_instance
            {
                set { this._all_instance = value; }
                get { return this._all_instance; }
            }

            public List<point2d> all_points
            {
                set { this._all_points = value; }
                get { return this._all_points; }
            }

            public List<edge2d> all_edges
            {
                set { this._all_edges = value; }
                get { return this._all_edges; }
            }

            public List<face2d> all_faces
            {
                set { this._all_faces = value; }
                get { return this._all_faces; }
            }

            public planar_object_store()
            {
                // Empty constructor used to initialize and re-intialize
                this._all_points = new List<point2d>();
                this._all_edges = new List<edge2d>();
                this._all_instance = new List<instance_tracker>();
            }

            public void paint_me(ref Graphics gr1, ref bool is_animate, ref int instance_counter)
            {
                Graphics gr0 = gr1;
                if (is_animate == true & instance_counter >= 0 & all_instance.Count != 0)
                {
                    all_instance[instance_counter].edge_list.ForEach(obj => obj.paint_me(ref gr0)); // Paint the edges
                    all_instance[instance_counter].face_list.ForEach(obj => obj.paint_me(ref gr0)); // Paint the faces
                }
                else
                {
                    all_edges.ForEach(obj => obj.paint_me(ref gr0)); // Paint the edges
                    all_faces.ForEach(obj => obj.paint_me(ref gr0)); // Paint the faces
                }

                all_points.ForEach(obj => obj.paint_me(ref gr0)); // Paint the points
            }

            public class instance_tracker
            {
                List<edge2d> _edge_list;
                List<face2d> _face_list;

                public List<edge2d> edge_list
                {
                    set { this._edge_list = value; }
                    get { return this._edge_list; }
                }

                public List<face2d> face_list
                {
                    set { this._face_list = value; }
                    get { return this._face_list; }
                }
            }

            public class point2d // class to store the points
            {
                int _id;
                double _x;
                double _y;

                public int id
                {
                    get { return this._id; }
                }

                public double x
                {
                    get { return this._x; }
                }
                public double y
                {
                    get { return this._y; }
                }
                public point2d(int i_id, double i_x, double i_y)
                {
                    // constructor 1
                    this._id = i_id;
                    this._x = i_x;
                    this._y = i_y;
                }

                public void paint_me(ref Graphics gr0) // this function is used to paint the points
                {
                    gr0.FillEllipse(new Pen(Color.BlueViolet, 2).Brush, new RectangleF(get_point_for_ellipse(), new SizeF(4, 4)));

                    if (the_static_class.ispaint_label == true)
                    {
                        string my_string = (this.id + 1).ToString() + "(" + this._x.ToString("F2") + ", " + this._y.ToString("F2") + ")";
                        SizeF str_size = gr0.MeasureString(my_string, new Font("Cambria", 6)); // Measure string size to position the dimension

                        gr0.DrawString(my_string, new Font("Cambria", 6),
                                                                           new Pen(Color.DarkBlue, 2).Brush,
                                                                           get_point_for_ellipse().X + 3 + the_static_class.to_single(-str_size.Width * 0.5),
                                                                           the_static_class.to_single(str_size.Height * 0.5) + get_point_for_ellipse().Y + 3);
                    }
                }

                public PointF get_point_for_ellipse()
                {
                    // y axis is flipped here
                    return (new PointF(the_static_class.to_single(this._x) - 2,
                   the_static_class.to_single((-1 * this._y) - 2))); // return the point as PointF as edge of an ellipse
                }

                public PointF get_point()
                {
                    // y axis is flipped here
                    return (new PointF(the_static_class.to_single(this._x),
                   the_static_class.to_single((-1 * this._y)))); // return the point as PointF as edge of an ellipse
                }

                public bool Equals(point2d other)
                {
                    return (this._x == other.x && this._y == other.y); // Equal function is used to check the uniqueness of the points added
                }
            }

            public class points_equality_comparer : IEqualityComparer<point2d>
            {
                public bool Equals(point2d a, point2d b)
                {
                    return (a.Equals(b));
                }

                public int GetHashCode(point2d other)
                {
                    return (other.x.GetHashCode() * 17 + other.y.GetHashCode() * 19);
                    // 17,19 are just ranfom prime numbers
                }
            }

            public class edge2d
            {
                int _edge_id;
                point2d _start_pt;
                point2d _end_pt;
                point2d _mid_pt; // not stored in point list

                public point2d start_pt
                {
                    get { return this._start_pt; }
                }

                public point2d end_pt
                {
                    get { return this._end_pt; }
                }

                public point2d mid_pt
                {
                    get { return this._mid_pt; }
                }

                public edge2d(int i_edge_id, point2d i_start_pt, point2d i_end_pt)
                {
                    // constructor 1
                    this._edge_id = i_edge_id;
                    this._start_pt = i_start_pt;
                    this._end_pt = i_end_pt;
                    this._mid_pt = new point2d(-1, (i_start_pt.x + i_end_pt.x) * 0.5, (i_start_pt.y + i_end_pt.y) * 0.5);
                }

                public void paint_me(ref Graphics gr0) // this function is used to paint the points
                {
                    Pen edge_pen = new Pen(Color.DarkOrange, 1);

                    gr0.DrawLine(edge_pen, start_pt.get_point(), end_pt.get_point());

                    //System.Drawing.Drawing2D.AdjustableArrowCap bigArrow = new System.Drawing.Drawing2D.AdjustableArrowCap(3, 3);
                    //edge_pen.CustomEndCap = bigArrow;
                    //gr0.DrawLine(edge_pen, start_pt.get_point(), mid_pt.get_point());
                }

                public bool Equals(edge2d other)
                {
                    return (other.start_pt.Equals(this._start_pt) && other.end_pt.Equals(this._end_pt));
                }

            }

            public class face2d
            {
                int _face_id;
                point2d _p1;
                point2d _p2;
                point2d _p3;
                point2d _mid_pt;
                double shrink_factor = 0.6f; //
                // in Circle
                point2d _circle_center;
                double _circle_radius;
                point2d _ellipse_edge;

                public int face_id
                {
                    get { return this._face_id; }
                }

                public PointF get_p1
                {
                    get
                    {
                        return new PointF(the_static_class.to_single(_mid_pt.get_point().X * (1 - shrink_factor) + (_p1.get_point().X * shrink_factor)),
                                           the_static_class.to_single(_mid_pt.get_point().Y * (1 - shrink_factor) + (_p1.get_point().Y * shrink_factor)));
                    }
                }

                public PointF get_p2
                {
                    get
                    {
                        return new PointF(the_static_class.to_single(_mid_pt.get_point().X * (1 - shrink_factor) + (_p2.get_point().X * shrink_factor)),
                                          the_static_class.to_single(_mid_pt.get_point().Y * (1 - shrink_factor) + (_p2.get_point().Y * shrink_factor)));
                    }
                }

                public PointF get_p3
                {
                    get
                    {
                        return new PointF(the_static_class.to_single(_mid_pt.get_point().X * (1 - shrink_factor) + (_p3.get_point().X * shrink_factor)),
                                          the_static_class.to_single(_mid_pt.get_point().Y * (1 - shrink_factor) + (_p3.get_point().Y * shrink_factor)));
                    }
                }

                public int get_p1_id
                {
                    get
                    {
                        return _p1.id;
                    }
                }
                public int get_p2_id
                {
                    get
                    {
                        return _p2.id;
                    }
                }
                public int get_p3_id
                {
                    get
                    {
                        return _p3.id;
                    }
                }
                public face2d(int i_face_id, point2d i_p1, point2d i_p2, point2d i_p3)
                {
                    this._face_id = i_face_id;
                    this._p1 = i_p1;
                    this._p2 = i_p2;
                    this._p3 = i_p3;
                    this._mid_pt = new point2d(-1, (i_p1.x + i_p2.x + i_p3.x) / 3, (i_p1.y + i_p2.y + i_p3.y) / 3);

                    set_incircle();

                }

                private void set_incircle()
                {
                    double dA = (this._p1.x * this._p1.x) + (this._p1.y * this._p1.y);
                    double dB = (this._p2.x * this._p2.x) + (this._p2.y * this._p2.y);
                    double dC = (this._p3.x * this._p3.x) + (this._p3.y * this._p3.y);

                    double aux1 = (dA * (this._p3.y - this._p2.y) + dB * (this._p1.y - this._p3.y) + dC * (this._p2.y - this._p1.y));
                    double aux2 = -(dA * (this._p3.x - this._p2.x) + dB * (this._p1.x - this._p3.x) + dC * (this._p2.x - this._p1.x));
                    double div = (2 * (this._p1.x * (this._p3.y - this._p2.y) + this._p2.x * (this._p1.y - this._p3.y) + this._p3.x * (this._p2.y - this._p1.y)));

                    if (div != 0)
                    {

                    }

                    //Circumcircle
                    double center_x = aux1 / div;
                    double center_y = aux2 / div;

                    this._circle_center = new point2d(-1, center_x, center_y);
                    this._circle_radius = Math.Sqrt((center_x - this._p1.x) * (center_x - this._p1.x) + (center_y - this._p1.y) * (center_y - this._p1.y));
                    this._ellipse_edge = new point2d(-1, center_x - this._circle_radius, center_y + this._circle_radius);

                }


                public void paint_me(ref Graphics gr0) // this function is used to paint the points
                {
                    Pen triangle_pen = new Pen(Color.LightGreen, 1);

                    if (the_static_class.is_paint_mesh == true)
                    {
                        PointF[] curve_pts = { get_p1, get_p2, get_p3 };
                        gr0.FillPolygon(triangle_pen.Brush, curve_pts); // Fill the polygon

                        if (the_static_class.ispaint_label == true)
                        {
                            string my_string = this._face_id.ToString();
                            SizeF str_size = gr0.MeasureString(my_string, new Font("Cambria", 6)); // Measure string size to position the dimension

                            gr0.DrawString(my_string, new Font("Cambria", 6), new Pen(Color.DeepPink, 2).Brush, this._mid_pt.get_point());

                        }
                    }

                    if (the_static_class.is_paint_incircle == true)
                    {
                        gr0.DrawEllipse(triangle_pen, this._ellipse_edge.get_point().X,
                                                         this._ellipse_edge.get_point().Y,
                                                         the_static_class.to_single(this._circle_radius * 2),
                                                         the_static_class.to_single(this._circle_radius * 2));
                    }

                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button_points_Click(object sender, EventArgs e)
        {
            if (the_static_class.test_a_textboxvalue_validity_int(textBox_input.Text, true, true) == true) // check whether the input (from the textbox) is valid or not
            {
                int inpt_point_count = Convert.ToInt32(textBox_input.Text);
                if (inpt_point_count > 2) // check to confirm the point count values
                {
                    // Generate random points inside the current canvas boundary
                    generate_random_points(inpt_point_count, (int)(the_static_class.canvas_size.Width * 0.5), (int)(the_static_class.canvas_size.Height * 0.5));
                }
            }
            else
            {
                // Prompt user to correct the input
                MessageBox.Show("Check the input !! No zero, decimal or negative inputs allowed",
               "Input Error",
               MessageBoxButtons.OK,
               MessageBoxIcon.Exclamation,
               MessageBoxDefaultButton.Button1);
            }

            if (the_static_class.is_animate_checked == true)
            {
                the_static_class.instance_counter_at = -1;
                timer1.Enabled = false;
            }


            mt_pic.Refresh();// Refresh the paint region
        }

        private void button_delaunay_Click(object sender, EventArgs e)
        {
            // start of the delaunay triangulation 
            main_drw_obj.all_edges = new List<planar_object_store.edge2d>(); // reinitialize the edge lists
            main_drw_obj.all_instance = new List<planar_object_store.instance_tracker>(); // reinitialize the instances lists
                                                                                          //the_static_class.instance_counter_at = -1;

            List<planar_object_store.edge2d> temp_edges = new List<planar_object_store.edge2d>();
            List<planar_object_store.face2d> temp_faces = new List<planar_object_store.face2d>();
            List<planar_object_store.instance_tracker> temp_instances = new List<planar_object_store.instance_tracker>();

            //(new delaunay_divide_n_conquer()).delaunay_start(main_drw_obj.all_points, ref temp_edges, ref temp_instances);
            // Delaunay Triangulation 
            (new delaunay_triangulation_divide_n_conquer()).delaunay_start(main_drw_obj.all_points, ref temp_edges, ref temp_faces, ref temp_instances);

            main_drw_obj.all_edges = temp_edges;
            main_drw_obj.all_faces = temp_faces;
            main_drw_obj.all_instance = temp_instances;

            //the_static_class.instance_counter_at = 0;
            if (the_static_class.is_animate_checked == true)
            {
                timer1.Enabled = true; // Timer to control the animation display
            }

            mt_pic.Refresh();// Refresh the paint region
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Timer tick
            if (the_static_class.instance_counter_at == main_drw_obj.all_instance.Count - 1)
            {
                the_static_class.instance_counter_at = 0;
            }
            else
            {
                the_static_class.instance_counter_at++;
            }
            mt_pic.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set canvas size when form loads
            main_drw_obj = new planar_object_store(); // intialize the main drawing object
            initiate_canvas_size();

            //(int)(the_static_class.canvas_size.Width * 0.5); // limits to create x random number
            //(int)(the_static_class.canvas_size.Height * 0.5); // limits to create y random number
            generate_random_points(10, (int)(the_static_class.canvas_size.Width * 0.5), (int)(the_static_class.canvas_size.Height * 0.5)); // Generate 10 Random points when the form loads
        }

        public void generate_random_points(int inpt_point_count, int x_coord_limit, int y_coord_limit)
        {
            main_drw_obj = new planar_object_store(); // reinitialize the all the lists
            List<planar_object_store.point2d> temp_pt_list = new List<planar_object_store.point2d>(); // create a temporary list to store the points                                                                                                     // !!!!!!!!!!!! Need major improvements below - very slow to generate unique n random points !!!!!!!!!!!!!!!!!!!!!!!!!!!
            int point_count = inpt_point_count;
            do
            {
                for (int i = 0; i < point_count; i++) // Loop thro' the point count
                {
                    planar_object_store.point2d temp_pt; // temp_pt to store the intermediate random points
                    PointF rand_pt = new PointF(rand0.Next(-x_coord_limit, x_coord_limit),
                                 rand0.Next(-y_coord_limit, y_coord_limit));
                    temp_pt = new planar_object_store.point2d(i, rand_pt.X / Math.Sqrt(2), rand_pt.Y / Math.Sqrt(2));
                    temp_pt_list.Add(temp_pt); // add to the temp list
                }

                temp_pt_list = temp_pt_list.Distinct(new planar_object_store.points_equality_comparer()).ToList();
                point_count = inpt_point_count - temp_pt_list.Count;
            } while (point_count != 0);
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // copy to the main list
            main_drw_obj.all_points = temp_pt_list;
            // List<PointF> temp_rand_pts = Enumerable.Range(0,point_count).Select(obj => the_static_class.random_point(-x_coord_limit, x_coord_limit,-y_coord_limit, y_coord_limit)).ToList();
        }
        public void generate_file_points(float[] points, int x_coord_limit, int y_coord_limit)
        {
            main_drw_obj = new planar_object_store(); // reinitialize the all the lists
            List<planar_object_store.point2d> temp_pt_list = new List<planar_object_store.point2d>(); // create a temporary list to store the points                                                                                                     // !!!!!!!!!!!! Need major improvements below - very slow to generate unique n random points !!!!!!!!!!!!!!!!!!!!!!!!!!!
            int point_count = points.Length/2;
            do
            {
                for (int i = 0; i < point_count; i++) // Loop thro' the point count
                {
                    planar_object_store.point2d temp_pt; // temp_pt to store the intermediate random points
                    PointF rand_pt = new PointF(points[(2*i)], points[(2*i) + 1]);
                    temp_pt = new planar_object_store.point2d(i, rand_pt.X , rand_pt.Y );
                    temp_pt_list.Add(temp_pt); // add to the temp list
                }

                temp_pt_list = temp_pt_list.Distinct(new planar_object_store.points_equality_comparer()).ToList();
                point_count = points.Length / 2 - temp_pt_list.Count;
            } while (point_count != 0);
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // copy to the main list
            main_drw_obj.all_points = temp_pt_list;
            // List<PointF> temp_rand_pts = Enumerable.Range(0,point_count).Select(obj => the_static_class.random_point(-x_coord_limit, x_coord_limit,-y_coord_limit, y_coord_limit)).ToList();
        }
        public void initiate_canvas_size()
        {
            // set canvas size and canvas orgin when the form loads and form size changes
            the_static_class.canvas_size = new SizeF(the_static_class.to_single(main_pic.Width * 0.8), the_static_class.to_single(main_pic.Height * 0.8));
            the_static_class.canvas_orgin = new PointF(the_static_class.to_single(main_pic.Width * 0.5), the_static_class.to_single(main_pic.Height * 0.5));
            mt_pic.Refresh(); // Refresh the paint region
        }

        private void main_pic_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality; // Paint high quality
            e.Graphics.TranslateTransform(the_static_class.canvas_orgin.X, the_static_class.canvas_orgin.Y); // Translate transform to make the orgin as center
            e.Graphics.ScaleTransform(the_static_class.to_single(Math.Sqrt(2)), the_static_class.to_single(Math.Sqrt(2)));

            Graphics gr1 = e.Graphics;

            // Draw orgin
            // e.Graphics.DrawEllipse(new Pen(Color.Black, 1), -2, -2, 4, 4);

            main_drw_obj.paint_me(ref gr1, ref the_static_class.is_animate_checked, ref the_static_class.instance_counter_at);
        }

        private void main_pic_SizeChanged(object sender, EventArgs e)
        {
            initiate_canvas_size();
        }

        public class the_static_class // this class stores all the useful functions and static variables
        {
            public static SizeF canvas_size; // size of the canvas (main_pic) or external bounds
            public static PointF canvas_orgin; // orgin of the canvas (mainpic) or centre point of external bounds

            public static bool ispaint_label = false; // static variable to control whether to paint id or not

            public static bool is_animate_checked = false; // static variable to control animation timing;
            public static bool is_paint_incircle = false;
            public static bool is_paint_mesh = false;

            public static int instance_counter_at; // static variable to control instances count
            public static int inpt_timer_interval = 500; // 0.5 seconds, static variable to control the interval of the timer

            /// <summary>
                        /// Function to generate random integer between a maximum and minimum value
                        /// </summary>
                        /// <param name="min"></param>
                        /// <param name="max"></param>
                        /// <returns></returns>
            //public static PointF random_point(int xmin, int xmax, int ymin, int ymax)
            //{
            //    Random rand_int = new Random();
            //    return (new PointF(rand_int.Next(xmin, xmax), rand_int.Next(ymin, ymax)));
            //}

            /// <summary>
                        /// Function to Check the valid of Numerical text from textbox.text
                        /// </summary>
                        /// <param name="tB_txt">Textbox.text value</param>
                        /// <param name="Negative_check">Is negative number Not allowed (True) or allowed (False)</param>
                        /// <param name="zero_check">Is zero Not allowed (True) or allowed (False)</param>
                        /// <returns>Return the validity (True means its valid) </returns>
                        /// <remarks></remarks>
            public static bool test_a_textboxvalue_validity_int(string tb_txt, bool n_chk, bool z_chk)
            {
                bool is_valid = false;
                //This function returns false if the textbox doesn't contains number 
                if (Int32.TryParse(tb_txt, out Int32 number) == true)
                {
                    is_valid = true;

                    if (n_chk == true) // check for negative number
                    {
                        if (Convert.ToInt32(tb_txt) < 0)
                        {
                            is_valid = false;
                        }
                    }

                    if (z_chk == true) // check for zero number
                    {
                        if (Convert.ToInt32(tb_txt) == 0)
                        {
                            is_valid = false;
                        }
                    }
                }
                return is_valid;
            }

            /// <summary>
                        /// Function to convert double to single (mostly used in System.Drawing functions)
                        /// </summary>
                        /// <param name="value"></param>
                        /// <returns></returns>
            public static float to_single(double value)
            {
                return (float)value;
            }

            /// <summary>
                        /// Funtion to check NAN or Infinity value
                        /// </summary>
                        /// <param name="chkval"></param>
                        /// <returns></returns>
            public static bool Isval_NAN_or_Infinity(double chkval)
            {
                return (double.IsNaN(chkval) || double.IsInfinity(chkval));
            }
        }

        #region "Check Box Events"
        private void checkBox_animate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_animate.Checked == true)
            {
                the_static_class.is_animate_checked = true;
                the_static_class.instance_counter_at = 0;
                timer1.Interval = the_static_class.inpt_timer_interval; // 0.5 seconds
                timer1.Enabled = true;
            }
            else
            {
                the_static_class.is_animate_checked = false;
                the_static_class.instance_counter_at = 0;
                timer1.Interval = the_static_class.inpt_timer_interval; // 0.5 seconds
                timer1.Enabled = false;
            }
            mt_pic.Refresh();
        }

        private void checkBox_coord_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_coord.Checked == true)
            {
                the_static_class.ispaint_label = true;
            }
            else
            {
                the_static_class.ispaint_label = false;
            }
            mt_pic.Refresh();
        }

        private void checkBox_incircle_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_incircle.Checked == true)
            {
                the_static_class.is_paint_incircle = true;
            }
            else
            {
                the_static_class.is_paint_incircle = false;
            }
            mt_pic.Refresh();
        }

        private void checkBox_mesh_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_mesh.Checked == true)
            {
                the_static_class.is_paint_mesh = true;
            }
            else
            {
                the_static_class.is_paint_mesh = false;
            }
            mt_pic.Refresh();
        }


        #endregion

        private void btnopenDialog_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textfilepath.Text = openFileDialog1.FileName;
            }
        }

        private void btnLoadPoints_Click(object sender, EventArgs e)
        {
            float scalefactor = 1;
            float.TryParse(txtScaleFactor.Text, out scalefactor);
            txtScaleFactor.Text = scalefactor.ToString();

            string path = textfilepath.Text;
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string alltext = sr.ReadToEnd();
                    alltext = alltext.Replace("\r", "");
                    alltext = alltext.Replace("\n", "");
                    string[] pointsstr = alltext.Split(',');
                    float[] points = new float[pointsstr.Length];
                    for (int i = 0; i < pointsstr.Length; i++)
                    {
                        bool succ = float.TryParse(pointsstr[i],out points[i]);
                        points[i] = points[i] * scalefactor;
                        if (!succ)
                        {
                            MessageBox.Show("file format is corrupted.");
                            return;
                        }
                    }
                    generate_file_points(points, (int)10, (int)10);
                    mt_pic.Refresh();// Refresh the paint region
                }
            }
            else
            {
                MessageBox.Show("file doesnt exist");
            }
                  }

        private void export_faces()
        {
            using (StreamWriter sw = File.CreateText("faces.txt"))
            {
                sw.WriteLine("");
                foreach (var item in main_drw_obj.all_faces)
                {
                    sw.Write(item.get_p1_id+1);
                    sw.Write(",");
                    sw.Write(item.get_p2_id+1);
                    sw.Write(",");
                    sw.WriteLine(item.get_p3_id+1);
                }
                MessageBox.Show("faces exported to faces.txt");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            export_faces();
        }
    }

}

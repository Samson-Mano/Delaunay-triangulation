namespace Delaunay_triangulation
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_input = new System.Windows.Forms.TextBox();
            this.button_points = new System.Windows.Forms.Button();
            this.button_delaunay = new System.Windows.Forms.Button();
            this.checkBox_animate = new System.Windows.Forms.CheckBox();
            this.main_pic = new System.Windows.Forms.Panel();
            this.mt_pic = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox_coord = new System.Windows.Forms.CheckBox();
            this.checkBox_incircle = new System.Windows.Forms.CheckBox();
            this.checkBox_mesh = new System.Windows.Forms.CheckBox();
            this.main_pic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mt_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of points :";
            // 
            // textBox_input
            // 
            this.textBox_input.Location = new System.Drawing.Point(128, 29);
            this.textBox_input.Name = "textBox_input";
            this.textBox_input.Size = new System.Drawing.Size(57, 23);
            this.textBox_input.TabIndex = 1;
            this.textBox_input.Text = "10";
            // 
            // button_points
            // 
            this.button_points.Location = new System.Drawing.Point(198, 28);
            this.button_points.Name = "button_points";
            this.button_points.Size = new System.Drawing.Size(112, 24);
            this.button_points.TabIndex = 2;
            this.button_points.Text = "Generate Points";
            this.button_points.UseVisualStyleBackColor = true;
            this.button_points.Click += new System.EventHandler(this.button_points_Click);
            // 
            // button_delaunay
            // 
            this.button_delaunay.Location = new System.Drawing.Point(316, 29);
            this.button_delaunay.Name = "button_delaunay";
            this.button_delaunay.Size = new System.Drawing.Size(112, 23);
            this.button_delaunay.TabIndex = 3;
            this.button_delaunay.Text = "Triangulate";
            this.button_delaunay.UseVisualStyleBackColor = true;
            this.button_delaunay.Click += new System.EventHandler(this.button_delaunay_Click);
            // 
            // checkBox_animate
            // 
            this.checkBox_animate.AutoSize = true;
            this.checkBox_animate.Location = new System.Drawing.Point(439, 31);
            this.checkBox_animate.Name = "checkBox_animate";
            this.checkBox_animate.Size = new System.Drawing.Size(72, 19);
            this.checkBox_animate.TabIndex = 4;
            this.checkBox_animate.Text = "Animate";
            this.checkBox_animate.UseVisualStyleBackColor = true;
            this.checkBox_animate.CheckedChanged += new System.EventHandler(this.checkBox_animate_CheckedChanged);
            // 
            // main_pic
            // 
            this.main_pic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_pic.BackColor = System.Drawing.Color.White;
            this.main_pic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.main_pic.Controls.Add(this.mt_pic);
            this.main_pic.Location = new System.Drawing.Point(12, 68);
            this.main_pic.Name = "main_pic";
            this.main_pic.Size = new System.Drawing.Size(760, 481);
            this.main_pic.TabIndex = 5;
            this.main_pic.SizeChanged += new System.EventHandler(this.main_pic_SizeChanged);
            this.main_pic.Paint += new System.Windows.Forms.PaintEventHandler(this.main_pic_Paint);
            // 
            // mt_pic
            // 
            this.mt_pic.BackColor = System.Drawing.Color.Transparent;
            this.mt_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mt_pic.Enabled = false;
            this.mt_pic.Location = new System.Drawing.Point(0, 0);
            this.mt_pic.Name = "mt_pic";
            this.mt_pic.Size = new System.Drawing.Size(756, 477);
            this.mt_pic.TabIndex = 0;
            this.mt_pic.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBox_coord
            // 
            this.checkBox_coord.AutoSize = true;
            this.checkBox_coord.Location = new System.Drawing.Point(517, 31);
            this.checkBox_coord.Name = "checkBox_coord";
            this.checkBox_coord.Size = new System.Drawing.Size(56, 19);
            this.checkBox_coord.TabIndex = 6;
            this.checkBox_coord.Text = "Label";
            this.checkBox_coord.UseVisualStyleBackColor = true;
            this.checkBox_coord.CheckedChanged += new System.EventHandler(this.checkBox_coord_CheckedChanged);
            // 
            // checkBox_incircle
            // 
            this.checkBox_incircle.AutoSize = true;
            this.checkBox_incircle.Location = new System.Drawing.Point(593, 31);
            this.checkBox_incircle.Name = "checkBox_incircle";
            this.checkBox_incircle.Size = new System.Drawing.Size(69, 19);
            this.checkBox_incircle.TabIndex = 7;
            this.checkBox_incircle.Text = "InCircle";
            this.checkBox_incircle.UseVisualStyleBackColor = true;
            this.checkBox_incircle.CheckedChanged += new System.EventHandler(this.checkBox_incircle_CheckedChanged);
            // 
            // checkBox_mesh
            // 
            this.checkBox_mesh.AutoSize = true;
            this.checkBox_mesh.Location = new System.Drawing.Point(668, 31);
            this.checkBox_mesh.Name = "checkBox_mesh";
            this.checkBox_mesh.Size = new System.Drawing.Size(56, 19);
            this.checkBox_mesh.TabIndex = 8;
            this.checkBox_mesh.Text = "Mesh";
            this.checkBox_mesh.UseVisualStyleBackColor = true;
            this.checkBox_mesh.CheckedChanged += new System.EventHandler(this.checkBox_mesh_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.checkBox_mesh);
            this.Controls.Add(this.checkBox_incircle);
            this.Controls.Add(this.checkBox_coord);
            this.Controls.Add(this.main_pic);
            this.Controls.Add(this.checkBox_animate);
            this.Controls.Add(this.button_delaunay);
            this.Controls.Add(this.button_points);
            this.Controls.Add(this.textBox_input);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(200, 600);
            this.Name = "Form1";
            this.Text = "Divide and Conquer - Delaunay Triangulation";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.main_pic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mt_pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_input;
        private System.Windows.Forms.Button button_points;
        private System.Windows.Forms.Button button_delaunay;
        private System.Windows.Forms.CheckBox checkBox_animate;
        private System.Windows.Forms.Panel main_pic;
        private System.Windows.Forms.PictureBox mt_pic;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBox_coord;
        private System.Windows.Forms.CheckBox checkBox_incircle;
        private System.Windows.Forms.CheckBox checkBox_mesh;
    }
}


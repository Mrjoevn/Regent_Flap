using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomMessageBoxDemo
{
    public partial class FrmMessageBox : Form
    {
        private Label lblTitle;
        private Label lblMessage;
        private Button btnOK;
        private Button btnCancel;
        private PictureBox picIcon;

        public FrmMessageBox(string title, string message)
        {
            InitializeComponent();
            lblTitle.Text = title;
            lblMessage.Text = message;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblMessage = new Label();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.picIcon = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // FrmMessageBox
            // 
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.ClientSize = new Size(360, 180);
            this.Text = "MessageBox";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            this.lblTitle.Location = new Point(90, 20);
            this.lblTitle.Size = new Size(250, 25);
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new Font("Segoe UI", 10F);
            this.lblMessage.ForeColor = Color.Black;
            this.lblMessage.Location = new Point(90, 60);
            this.lblMessage.Size = new Size(250, 60);
            // 
            // picIcon
            // 
            this.picIcon.Location = new Point(25, 50);
            this.picIcon.Size = new Size(48, 48);
            this.picIcon.Image = SystemIcons.Information.ToBitmap();
            this.picIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            // 
            // btnOK
            // 
            this.btnOK.Text = "OK";
            this.btnOK.BackColor = Color.FromArgb(52, 152, 219);
            this.btnOK.ForeColor = Color.White;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.Size = new Size(100, 35);
            this.btnOK.Location = new Point(70, 130);
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Text = "Cancel";
            this.btnCancel.BackColor = Color.FromArgb(189, 195, 199);
            this.btnCancel.ForeColor = Color.Black;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Location = new Point(190, 130);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            // 
            // Controls
            // 
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.picIcon);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.ResumeLayout(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int radius = 15;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);
        }
    }
}

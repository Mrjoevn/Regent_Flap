using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace FLap_New.SubForm
{
    public partial class frmFabricInMes : Form
    {
        public frmFabricInMes()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmFabricInMes_Load(object sender, EventArgs e)
        {
            MetroFramework.Controls.MetroButton btn = new MetroFramework.Controls.MetroButton();
            btn.Text = "Click Me!";
            btn.Location = new Point(50, 50);
            btn.Size = new Size(100, 30);
            this.Controls.Add(btn);
        }
    }
}

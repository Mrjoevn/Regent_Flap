using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FLap_New.SubForm;

namespace FLap_New
{
    public partial class frmIndex : Form
    {
        public frmIndex()
        {
            InitializeComponent();
            #region IconInControl
            btnCutting.Controls.Add(pcbCutting);
            pcbCutting.Location = new Point(5, 5);
            btnVap.Controls.Add(pcbVap);
            pcbVap.Location = new Point(5, 5);
            btnReport.Controls.Add(pcbReport);
            pcbReport.Location = new Point(5, 5);
            btnDashboard.Controls.Add(pcbDashboard);
            pcbDashboard.Location = new Point(5, 5);
            btnExit.Controls.Add(pcbExit);
            pcbExit.Location = new Point(5, 5);
            #endregion IconInControl
            SubNavigation_Hide();
        }

        private void Index_Load(object sender, EventArgs e)
        {
            //frmInterface inter = new frmInterface();
            //inter.Show();
            //showDashBoard(pnContent);
        }
        #region Navigation
        private void SubNavigation_Hide()
        {
            pnCctSubnav.Visible = false;
            pnVapSubnav.Visible = false;
            pnReportSubnav.Visible = false;
        }
        private void hideSubmenu()
        {
            if(pnCctSubnav.Visible)
                pnCctSubnav.Visible = false;
            if(pnVapSubnav.Visible)
                pnVapSubnav.Visible = false;
            if( pnReportSubnav.Visible)
                pnReportSubnav.Visible = false;
        }
        private void showSubmenu(Panel submenu)
        {
            if(submenu.Visible == false)
            {
                hideSubmenu();
                submenu.Visible = true;
            } else
                submenu.Visible = false;
        }

        private void btnCutting_Click(object sender, EventArgs e)
        {
            showSubmenu(pnCctSubnav);
        }

        private void btnVap_Click(object sender, EventArgs e)
        {
            showSubmenu(pnVapSubnav);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            showSubmenu(pnReportSubnav);
        }
        #endregion Navigation

        private void btnExit_Click(object sender, EventArgs e)
        {
            //changeBackGroundBtn(btnExit);
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn Thoát .","Xác nhận",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result == DialogResult.Yes) 
                this.Close();
        }
        private void changeBackGroundBtn(Button btn)
        {
            btn.BackColor = Color.FromArgb(244, 244, 244);
            btn.ForeColor = Color.Black;
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmInterface frm = new frmInterface();
            showDashBoard(frm,"DashBoard");
        }
        public void showDashBoard(Form frm,string title)
        {
            #region subform
            //// Check if the form is already open
            //foreach (Control ctrl in pnContent.Controls)
            //{
            //    if (ctrl is Form existingForm && existingForm.Name == frm.Name)
            //    {
            //        existingForm.BringToFront();
            //        return;
            //    }
            //}
            //frm.TopLevel = false;       // Bắt buộc để nhúng vào panel
            //frm.FormBorderStyle = FormBorderStyle.None; // Xóa viền
            //frm.Dock = DockStyle.Fill;  // Phủ đầy panel
            ////pnContent.Controls.Clear(); // Xóa control cũ nếu có
            //pnContent.Controls.Add(frm);
            //frm.Show();

            //// --- Create navigation button ---
            //Button navBtn = new Button();
            //navBtn.Text = title;
            //navBtn.Tag = frm; // Store form reference
            //navBtn.AutoSize = true;
            //navBtn.Margin = new Padding(5);
            //navBtn.Click += (s, e) =>
            //{
            //    // When clicked, bring this form to front
            //    frm.BringToFront();
            //};

            //pnMenuBar.Controls.Add(navBtn);
            #endregion subform
            // 1️⃣ Check if this form is already open
            foreach (Control ctrl in pnContent.Controls)
            {
                if (ctrl is Form existingForm && existingForm.Name == frm.Name)
                {
                    existingForm.BringToFront();
                    return;
                }
            }

            // 2️⃣ Prepare subform (embed inside container)
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            pnContent.Controls.Add(frm);
            frm.Show();

            //// 3️⃣ Create navigation button
            //Button navBtn = new Button();
            //navBtn.Text = title;
            //navBtn.Tag = frm; // store reference to form
            //navBtn.AutoSize = true;
            //navBtn.Margin = new Padding(5);
            //navBtn.BackColor = Color.LightGray;
            //navBtn.Click += (s, e) =>
            //{
            //    frm.BringToFront();
            //    HighlightNavButton(navBtn);
            //};

            //// 4️⃣ Add button to navigation panel
            //pnMenuBar.Controls.Add(navBtn);

            //// 5️⃣ When the form closes, remove its button
            //frm.FormClosed += (s, e) =>
            //{
            //    pnMenuBar.Controls.Remove(navBtn);
            //    navBtn.Dispose();
            //};
            // --- Tạo Panel chứa nút nav + nút close ---
            Panel navItem = new Panel();
            navItem.Height = 35;
            navItem.Width = 100;
            navItem.Margin = new Padding(3);
            navItem.BackColor = Color.LightGray;

            // --- Nút chính (click để chuyển form) ---
            Button btnMain = new Button();
            btnMain.Text = title;
            btnMain.Dock = DockStyle.Fill;
            btnMain.FlatStyle = FlatStyle.Flat;
            btnMain.FlatAppearance.BorderSize = 0;
            btnMain.Tag = frm;
            btnMain.Click += (s, e) =>
            {
                frm.BringToFront();
                HighlightNavPanel(navItem);
            };

            // --- Nút close nhỏ ---
            Button btnClose = new Button();
            btnClose.Text = "×";
            btnClose.Width = 25;
            btnClose.Dock = DockStyle.Right;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.ForeColor = Color.Red;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Click += (s, e) =>
            {
                frm.Close(); // tự động kích hoạt sự kiện FormClosed bên dưới
            };

            // --- Ghép vào panel nav item ---
            navItem.Controls.Add(btnClose);
            navItem.Controls.Add(btnMain);
            pnMenuBar.Controls.Add(navItem);

            // --- Khi form đóng thì remove nav item ---
            frm.FormClosed += (s, e) =>
            {
                pnMenuBar.Controls.Remove(navItem);
                navItem.Dispose();
            };

            HighlightNavPanel(navItem);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); // triggers the FormClosed event from main form
        }

        private Panel activePanel = null;
        private void HighlightNavPanel(Panel p)
        {
            if (activePanel != null)
                activePanel.BackColor = Color.LightGray;

            activePanel = p;
            activePanel.BackColor = Color.DodgerBlue;
        }
    }
}

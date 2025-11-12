using FLap_New.CCT_SubForm;
using FLap_New.VAP_SubForm;
using FLap_New.SubForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
            frmInterface frm = new frmInterface();
            showInterface(frm, "DashBoard");
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
            showInterface(frm,"DashBoard");
        }
        public void showInterface(Form frm,string title)
        {
            int closeWidth = 25;
            int navContentWidth = 100;
            // 1️⃣ Check if this form is already open
            foreach (Control ctrl in pnContent.Controls)
            {
                if (ctrl is Form existingForm && existingForm.Name == frm.Name)
                {
                    existingForm.BringToFront();

                    // 🔹 Tìm navItem tương ứng để highlight
                    foreach (Control nav in pnMenuBar.Controls)
                    {
                        if (nav is Panel navItemPanel)
                        {
                            // Kiểm tra trong panel có Button nào trỏ tới form này không
                            foreach (Control btn in navItemPanel.Controls)
                            {
                                if (btn is Button b && b.Tag == existingForm)
                                {
                                    HighlightNavPanel(navItemPanel);
                                    break;
                                }
                            }
                        }
                    }

                    return;
                }
            }

            // 2️⃣ Prepare subform (embed inside container)
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            frm.Width = 1268;
            pnContent.Controls.Add(frm);
            frm.Show();
            frm.BringToFront();
            // --- Tạo Panel chứa nút nav + nút close ---
            Panel navItem = new Panel();
            navItem.Height = 30;
            navItem.Width = navContentWidth;
            //navItem.BackColor = Color.LightSlateGray;
            navItem.Margin = new Padding(0,0,2,0);

            // --- Nút chính (click để chuyển form) ---
            Button btnMain = new Button
            {
                Text = title,
                Dock = DockStyle.Left,
                Width = navContentWidth - closeWidth,
                FlatStyle = FlatStyle.Flat,
                Padding = new Padding(0, 4, 0, 0),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                //BackColor = Color.DarkGray,
                //ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = frm
            };
            btnMain.FlatAppearance.BorderSize = 0;
            btnMain.Click += (s, e) =>
            {
                frm.BringToFront();
                HighlightNavPanel(navItem);
            };

            // --- Nút close riêng cho form này ---
            Button btnClose = new Button
            {
                Text = "x",
                TextAlign = ContentAlignment.MiddleCenter,
                Width = closeWidth,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                //BackColor = Color.DarkGray,
                //ForeColor = Color.White,
                Tag = frm // Gắn form tương ứng để dễ xử lý
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) =>
            {
                Form f = (Form)((Button)s).Tag;
                f.Close(); // Gọi sự kiện FormClosed tương ứng
            };

            // --- Ghép control ---
            navItem.Controls.Add(btnClose);
            navItem.Controls.Add(btnMain);
            pnMenuBar.Controls.Add(navItem);

            // 4️⃣ Khi form đóng → tự xóa nav item
            frm.FormClosed += (s, e) =>
            {
                if (pnMenuBar.Controls.Contains(navItem))
                {
                    pnMenuBar.Controls.Remove(navItem);
                }
                if (pnContent.Controls.Contains(frm))
                {
                    pnContent.Controls.Remove(frm);
                }
                navItem.Dispose();
                frm.Dispose();
            };
            HighlightNavPanel(navItem);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); // triggers the FormClosed event from main form
        }
        //Highligh Form's button on taskbar
        private Panel activePanel = null;
        private void HighlightNavPanel(Panel p)
        {
            if (activePanel != null)
                activePanel.BackColor = Color.DarkGray;

            activePanel = p;
            activePanel.BackColor = Color.DodgerBlue;
        }

        private void btnCutting_Mes_Click(object sender, EventArgs e)
        {
            frmFabricInMes frmCctMes = new frmFabricInMes();
            showInterface(frmCctMes,"MES");
        }

        private void btnCutting_Cics_Click(object sender, EventArgs e)
        {
            frmFabricInCics frmCctCics = new frmFabricInCics();
            showInterface(frmCctCics,"CICS");
        }

        private void bnCutting_Cct_Click(object sender, EventArgs e)
        {
            frmCuttingRecord frmCctCut = new frmCuttingRecord();
            showInterface(frmCctCut, "CutRecord");
        }

        private void btnCutting_Mtm_Click(object sender, EventArgs e)
        {
            frmMtm frmmtm = new frmMtm();
            showInterface(frmmtm, "MTM");
        }

        private void btnRelaxing_Report_Click(object sender, EventArgs e)
        {
            frmRelaxingWebsite frmrlx = new frmRelaxingWebsite();
            showInterface(frmrlx, "Relaxing");
            ////string url = "http://192.168.40.41:7557/HomePage";
            //string url = "https://www.youtube.com/";

            //Process.Start(new ProcessStartInfo
            //{
            //    FileName = url,
            //    UseShellExecute = true // cần dòng này để mở bằng trình duyệt mặc định
            //});
        }

        private void btnVap_PrePanel_Click(object sender, EventArgs e)
        {
            frmDeliveryPanel frmDelivery = new frmDeliveryPanel();
            showInterface(frmDelivery, "Delivery");
        }
    }
}

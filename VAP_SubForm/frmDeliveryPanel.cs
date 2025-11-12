using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLap_New.VAP_SubForm
{
    public partial class frmDeliveryPanel : Form
    {
        public frmDeliveryPanel()
        {
            InitializeComponent();
        }
        #region StartDate
        private ToolStripDropDown dropDown;
        private MonthCalendar monthCalendar;
        private void ShowCalendar(TextBox targetTextBox, PictureBox picture)
        {
            // Nếu popup đang mở thì đóng lại
            if (dropDown != null && dropDown.Visible)
            {
                dropDown.Close();
                return;
            }

            monthCalendar = new MonthCalendar();
            monthCalendar.MaxSelectionCount = 1; // Chỉ cho chọn 1 ngày
            monthCalendar.DateSelected += (s, e) =>
            {
                targetTextBox.Text = e.Start.ToString("yyyy-MM-dd");
                dropDown.Close(); // Ẩn popup sau khi chọn
            };

            ToolStripControlHost host = new ToolStripControlHost(monthCalendar)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = monthCalendar.Size
            };

            dropDown = new ToolStripDropDown
            {
                Padding = Padding.Empty
            };
            dropDown.Items.Add(host);

            // Hiển thị lịch ngay dưới TextBox
            var location = picture.PointToScreen(new Point(0, picture.Height));
            dropDown.Show(location);
        }
        #endregion StartDate
        #region WaterMark
        private string watermarkText = "yyyy-mm-dd";
        private void SetupWatermark(TextBox textBox)
        {
            // Nếu ban đầu trống thì gán watermark
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = watermarkText;
                textBox.ForeColor = Color.Gray;
            }

            textBox.GotFocus += (s, e) =>
            {
                // Nếu đang hiển thị watermark thì xóa đi để nhập
                if (textBox.Text == watermarkText && textBox.ForeColor == Color.Gray)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                // Nếu người dùng không nhập gì thì gán lại watermark
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = watermarkText;
                    textBox.ForeColor = Color.Gray;
                }
                else
                {
                    textBox.ForeColor = Color.Black; // Giữ màu đen nếu người dùng có nhập
                }
            };
        }
        #endregion WaterMark
        private void btnMtmSearch_Click(object sender, EventArgs e)
        {

        }

        private void frmDeliveryPanel_Load(object sender, EventArgs e)
        {
            SetupWatermark(txtDelStartDate);
            SetupWatermark(txtDelEndDate);
            CustomizeGrid(dgvDelTrolleyInfo);
            CustomizeGrid(dgvDelQtyinTrolley);
            CustomizeGrid(dgvDelAllProcess);
            CustomizeGrid1();
            //Thêm 3 dòng trống
            for (int i = 0; i < 7; i++)
            {
                dgvDelReport.Rows.Add();
            }
        }
        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            ComboBox cb = sender as ComboBox;
            string text = cb.Items[e.Index].ToString();

            // Nền khi chọn
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            // Vẽ chữ
            TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, Color.Black, TextFormatFlags.Left);

            e.DrawFocusRectangle();
        }

        private void pcbMtmStartDate_Click(object sender, EventArgs e)
        {
            txtDelStartDate.Focus();
            ShowCalendar(txtDelStartDate, pcbDelStartDate);
        }

        private void pcbDelEndDate_Click(object sender, EventArgs e)
        {
            txtDelEndDate.Focus();
            ShowCalendar(txtDelEndDate, pcbDelEndDate);
        }

        private void pnDelContent_Resize(object sender, EventArgs e)
        {
            pnDelContent_Left.Width = pnDelContent.Width - pnDelContent_Right.Width;
            pnDelTrolleyInfo.Height = pnDelContent_Left.Height - pnDelQtyInTrolley.Height;
            pnDelAllProces.Height = pnDelContent_Right.Height - pnDelReport.Height;
        }

        private void CustomizeGrid(DataGridView dt)
        {
            var dgv = dt;

            // Tắt redraw để tránh lag khi render nhiều thay đổi
            dgv.SuspendLayout();

            // Font & màu sắc cơ bản
            dgv.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.GridColor = Color.FromArgb(230, 230, 230);
            dgv.BackgroundColor = Color.White;

            // Header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Dòng dữ liệu
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 240, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(3, 2, 3, 2);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False; // 🔹 tránh ngắt dòng khiến chữ bị ẩn

            // Dòng xen kẽ (alternate row)
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            // Kích thước tự động (chỉ theo nội dung, không fill toàn bảng)
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells; // 🔹 nhanh hơn Fill
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; // 🔹 tránh auto height gây lag
            // Không cho người dùng chỉnh sửa hoặc thêm dòng
            dgv.ReadOnly = false;
            dgv.AllowUserToAddRows = false;
            //dgv.AllowUserToResizeRows = false;

            // Chiều cao dòng vừa đủ cho font (tránh bị cắt chữ)
            dgv.RowTemplate.Height = 30;
            // Cuộn mượt hơn
            //dgv.DoubleBuffered(true); // 🔹 custom extension bên dưới
            dgv.ResumeLayout();
        }
        private void CustomizeGrid1()
        {
            var dgv = dgvDelReport;

            // Tắt redraw để tránh lag khi render nhiều thay đổi
            dgv.SuspendLayout();

            // Font & màu sắc cơ bản
            dgv.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.GridColor = Color.FromArgb(230, 230, 230);
            dgv.BackgroundColor = Color.White;

            // Header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            //dgv.ColumnHeadersHeight = 35;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Dòng dữ liệu
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 240, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(3, 2, 3, 2);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False; // 🔹 tránh ngắt dòng khiến chữ bị ẩn

            // Dòng xen kẽ (alternate row)
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            // Kích thước tự động (chỉ theo nội dung, không fill toàn bảng)
            //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells; // 🔹 nhanh hơn Fill
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; // 🔹 tránh auto height gây lag
            // Không cho người dùng chỉnh sửa hoặc thêm dòng
            dgv.ReadOnly = false;
            dgv.AllowUserToAddRows = false;
            //dgv.AllowUserToResizeRows = false;

            // Chiều cao dòng vừa đủ cho font (tránh bị cắt chữ)
            dgv.RowTemplate.Height = 30;
            // Cuộn mượt hơn
            //dgv.DoubleBuffered(true); // 🔹 custom extension bên dưới
            dgv.ResumeLayout();
        }
    }
}

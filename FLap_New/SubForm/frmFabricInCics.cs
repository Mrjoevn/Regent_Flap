﻿using FLap_New.Object;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FLap_New.SubForm
{
    public partial class frmFabricInCics : Form
    {
        static ConnectionToSql conn = new ConnectionToSql("CicsConnectionString");
        static string[] allConnection = conn.GetRawConnectionString();
        static string cicsConnect = allConnection[0];
        public frmFabricInCics()
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

        private void pcbCicsStartDate_Click(object sender, EventArgs e)
        {
            txtCiscStartDate.Focus();
            ShowCalendar(txtCiscStartDate, pcbCicsStartDate);
        }
        #region WaterMark
        private string watermarkText = "dd-mm-yyyy";
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

        private void frmFabricInCics_Load(object sender, EventArgs e)
        {
            SetupWatermark(txtCiscStartDate);
            SetupWatermark(txtCiscEndDate);
            CustomizeGrid1();
            CustomizeGrid2();
        }
        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            if (dgvCicsDetail.Rows.Count == 0)
            {
                string message = "❌ Không có dữ liệu";
                Font font = new Font("Segoe UI", 14, FontStyle.Bold);
                Color textColor = Color.FromArgb(231, 76, 60); // đỏ cam hiện đại
                SizeF textSize = e.Graphics.MeasureString(message, font);

                float x = (panel6.Width - textSize.Width) / 2;
                float y = (panel6.Height - textSize.Height) / 2;

                e.Graphics.DrawString(message, font, new SolidBrush(textColor), x, y);
            }
        }
        public void GetCicsData(List<string> data, List<string> para)
        {
            if (data.Count > 0)
            {
                pnCicsContent.Visible = true;
                using (SqlConnection sqlcon = new SqlConnection(cicsConnect))
                {
                    try
                    {
                        sqlcon.Open();
                        string storedProcName = "RP_checkfabricinfoCICS_dtl";
                        using (SqlCommand cmd = new SqlCommand(storedProcName, sqlcon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            for (int i = 0; i < data.Count; i++)
                            {
                                cmd.Parameters.AddWithValue(para[i], data[i]); // Ví dụ DeptId = 5
                            }
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvCicsDetail.DataSource = dt;
                        }
                        string storedProcName1 = "RP_checkfabricinfoCICS_mst";
                        using (SqlCommand cmd1 = new SqlCommand(storedProcName1, sqlcon))
                        {
                            cmd1.CommandType = CommandType.StoredProcedure;
                            for (int i = 0; i < data.Count; i++)
                            {
                                cmd1.Parameters.AddWithValue(para[i], data[i]); // Ví dụ DeptId = 5
                            }
                            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);

                            dgvCicsSumary.DataSource = dt1;
                        }
                        int rows = dgvCicsDetail.Rows.Count;
                        if (rows <= 0)
                        {
                            pnCicsContent.Visible = false;
                            dgvCicsDetail.DataSource = null;
                            dgvCicsDetail.Columns.Clear();
                            panel6.Paint += panel6_Paint;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập dữ liệu .", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCicsSo.Focus();
            }
        }
        private void btnCicsSearch_Click(object sender, EventArgs e)
        {
            List<string> data = new List<string>();
            List<string> para = new List<string>();
            if (!Equals(txtCicsBatch.Text.Trim(), ""))
            {
                string batch = txtCicsBatch.Text.Trim();
                data.Add(batch);
                string parameter = "@batch";
                para.Add(parameter);
            }
            if (!Equals(txtCicsSo.Text.Trim(), ""))
            {
                string so = txtCicsSo.Text.Trim();
                data.Add(so);
                string parameter = "@so";
                para.Add(parameter);
            }
            string cicsStartDate = (txtCiscStartDate.Text == watermarkText && txtCiscStartDate.ForeColor == Color.Gray) ? "" : txtCiscStartDate.Text;
            string cicsEndDate = (txtCiscEndDate.Text == watermarkText && txtCiscEndDate.ForeColor == Color.Gray) ? "" : txtCiscEndDate.Text;
            if (!Equals(cicsStartDate, ""))
            {
                data.Add(cicsStartDate);
                string parameter = "@startDate";
                para.Add(parameter);
            }
            if (!Equals(cicsEndDate, ""))
            {
                data.Add(cicsEndDate);
                string parameter = "@endDate";
                para.Add(parameter);
            }
            GetCicsData(data, para);
        }
        private void CustomizeGrid1()
        {
            var dgv = dgvCicsDetail;

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
        private void CustomizeGrid2()
        {
            var dgv = dgvCicsSumary;

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
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
            dgv.RowTemplate.Height = 33;
            // Cuộn mượt hơn
            //dgv.DoubleBuffered(true); // 🔹 custom extension bên dưới
            dgv.ResumeLayout();
        }

        private void pcbCicsEndDate_Click(object sender, EventArgs e)
        {
            txtCiscEndDate.Focus();
            ShowCalendar(txtCiscEndDate, pcbCicsEndDate);
        }
        public DataSet DataSetExportToExcelrlx()
        {
            DataSet rlx = new DataSet();
            //add dttb to dts
            var rlxmst = new DataTable();
            rlxmst = (DataTable)dgvCicsSumary.DataSource;
            var rlxdtl = new DataTable();
            rlxdtl = (DataTable)dgvCicsDetail.DataSource;
            if (rlxmst.Rows.Count > 0)
            {
                rlx.Tables.Add(rlxmst);
            }
            if (rlxdtl.Rows.Count > 0)
            {
                rlx.Tables.Add(rlxdtl);
            }
            return rlx;
        }
        private void btnCicsExport_Click(object sender, EventArgs e)
        {
            if (dgvCicsDetail.Rows.Count <= 0)
            {
                MessageBox.Show("Chưa có dữ liệu. Không thể Export", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }else
            {
                // Set the license context before using ExcelPackage
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                //lblcct.Visible = true;
                ////lblinform.Text = "select * from AQL_Per where line_code='" + txtGroup.Text + "' and Effect_Date between '" + calfrm + "' and '" + calto + "'";
                //lblcct.Text = " Thời gian quá gấp nên chức năng này chưa xong được nhé !";
                DataSet dtsmst = DataSetExportToExcelrlx();
                ExcelPackage excel = new ExcelPackage();
                string Exlfilename = "RLX Report";
                for (int k = 0; k < dtsmst.Tables.Count; k++)
                {
                    DataTable dttbmst = dtsmst.Tables[k];
                    var tbname = dttbmst.TableName;
                    var totalCols = dttbmst.Columns.Count;
                    var totalRows = dttbmst.Rows.Count;
                    var workSheet = excel.Workbook.Worksheets.Add(tbname);
                    //var characters = Funcrefer.Convertinttochar(dttb.Columns.Count - 1);
                    //var crt = Funcrefer.Convertinttochar(result.Columns.Count);
                    //workSheet.Cells["B1:" + characters + "1"].Merge = true;
                    workSheet.Cells[1, 1, 1, dttbmst.Columns.Count].Merge = true;

                    workSheet.Cells[1, 1].Value = "REGENT GARMENT FACTOTY LTD";
                    workSheet.Cells[3, 2].Value = "CCT Relaxing Information Report";
                    workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //workSheet.Cells["B2:" + characters + "2"].Merge = true;
                    workSheet.Cells[2, 1, 2, dttbmst.Columns.Count].Merge = true;
                    workSheet.Cells[2, 1].Value = "(A SUBSIDIARY OF CRYSTAL INTERNATIONAL GROUP LTD)";
                    workSheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //workSheet.Cells["B3:" + characters + "3"].Merge = true;
                    workSheet.Cells[3, 2, 3, dttbmst.Columns.Count - 1].Merge = true;
                    workSheet.Cells[3, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (totalCols < 6)
                    {
                        workSheet.Cells[1, 1].Style.Font.Size = 12;
                        workSheet.Cells[2, 1].Style.Font.Size = 11;
                        workSheet.Cells[3, 2].Style.Font.Size = 11;
                    }
                    else
                    {
                        workSheet.Cells[1, 1].Style.Font.Size = 16;
                        workSheet.Cells[2, 1].Style.Font.Size = 12;
                        workSheet.Cells[3, 2].Style.Font.Size = 12;
                    }
                    workSheet.Cells[1, 1].Style.Font.Bold = true;
                    workSheet.Cells[2, 1].Style.Font.Bold = true;
                    workSheet.Cells[3, 2].Style.Font.Bold = true;
                    workSheet.Cells[3, 1].Value = "Date: " + DateTime.Now.ToString("dd-MM-yyyy");
                    workSheet.Cells[4, 1].Value = "Time: " + DateTime.Now.ToString("hh:mm:ss tt");
                    //workSheet.Cells[3, dttb.Columns.Count].Value = "Fr: " + CalendarFrom.SelectedDate.ToString("dd-MM-yyyy");
                    //workSheet.Cells[4, dttb.Columns.Count].Value = "To: " + CalendarTo.SelectedDate.ToString("dd-MM-yyyy");
                    workSheet.Cells[3, 1].Style.Font.Italic = true;
                    workSheet.Cells[4, 1].Style.Font.Italic = true;
                    workSheet.Cells[3, dttbmst.Columns.Count].Style.Font.Italic = true;
                    workSheet.Cells[4, dttbmst.Columns.Count].Style.Font.Italic = true;
                    for (var col = 1; col <= totalCols; col++)
                    {
                        workSheet.Cells[5, col].Value = dttbmst.Columns[col - 1].ColumnName;
                        workSheet.Cells[5, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[5, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        workSheet.Cells[5, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[5, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[5, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[5, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[5, col].Style.Font.Bold = true;
                    }
                    for (var row = 1; row <= totalRows; row++)
                    {
                        for (var col = 0; col < totalCols; col++)
                        {
                            workSheet.Cells[row + 5, col + 1].Value = dttbmst.Rows[row - 1][col];
                        }
                    }
                    workSheet.Cells[5, 1, 5, dttbmst.Columns.Count].AutoFilter = true;
                    workSheet.View.FreezePanes(6, 1);
                    workSheet.Cells.AutoFitColumns();
                    workSheet.Protection.IsProtected = false;
                    workSheet.Protection.AllowSelectLockedCells = false;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save an Excel File";
                saveFileDialog.FileName = Exlfilename + "." + DateTime.Now.ToString("ddMMyyyy_hhmmtt") + ".xlsx";

                // Show the dialog and get the result
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string filePathmst = saveFileDialog.FileName;

                    // Save the Excel file to the specified path
                    using (var memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);
                        File.WriteAllBytes(filePathmst, memoryStream.ToArray());
                    }
                    //Open file path
                    Process.Start(new ProcessStartInfo(filePathmst) { UseShellExecute = true });
                    //MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

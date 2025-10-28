using FLap_New.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using OfficeOpenXml.Style;

namespace FLap_New.SubForm
{
    public partial class frmFabricInMes : Form
    {
        static ConnectionToSql conn = new ConnectionToSql("CicsConnectionString");
        static string[] allConnection = conn.GetRawConnectionString();
        static string mesConnect = allConnection[1];

        public frmFabricInMes()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmFabricInMes_Load(object sender, EventArgs e)
        {
            SetupWatermark(txtDateStart);
            SetupWatermark(txtDateEnd);
            //Console.WriteLine(pnMesContent.Size);
            //Console.WriteLine(dgvMesData.Size);
            // Bật double buffer giúp cuộn mượt hơn
            dgvMesData.SetDoubleBuffered(true);
            CustomizeGrid();
            //string userInput = (textBox1.Text == watermarkText && textBox1.ForeColor == Color.Gray) ? "" : textBox1.Text;
            //check Date value condition if value is what we define will skip it
        }
        private void pcbStartDate_Click(object sender, EventArgs e)
        {
            txtDateStart.Focus();
            ShowCalendar(txtDateStart,pcbStartDate);
        }
        #region StartDate
        private ToolStripDropDown dropDown;
        private MonthCalendar monthCalendar;
        private void ShowCalendar(TextBox targetTextBox,PictureBox picture)
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
        private void pcbEndDate_Click(object sender, EventArgs e)
        {
            txtDateEnd.Focus();
            ShowCalendar(txtDateEnd, pcbEndDate);
        }

        private void btnMesSearch_Click(object sender, EventArgs e)
        {
            Console.WriteLine(mesConnect);
            List<string> data = new List<string>();
            List<string> para = new List<string>();
            if (!Equals(txtMesBatch.Text.Trim(), ""))
            {
                string batch = txtMesBatch.Text.Trim();
                data.Add(batch);
                string parameter = "@batch";
                para.Add(parameter);
            }
            if (!Equals(txtMesSo.Text.Trim(), ""))
            {
                string so = txtMesSo.Text.Trim();
                data.Add(so);
                string parameter = "@so";
                para.Add(parameter);
            }
            if (!Equals(txtMesRoll.Text.Trim(), ""))
            {
                string roll = txtMesRoll.Text.Trim();
                data.Add(roll);
                string parameter = "@roll";
                para.Add(parameter);
            }
            if (!Equals(txtMesTrolley.Text.Trim(), ""))
            {
                string troll = txtMesTrolley.Text.Trim();
                data.Add(troll);
                string parameter = "@trolley";
                para.Add(parameter);
            }
            string startDate = (txtDateStart.Text == watermarkText && txtDateStart.ForeColor == Color.Gray) ? "" : txtDateStart.Text;
            string endDate = (txtDateEnd.Text == watermarkText && txtDateEnd.ForeColor == Color.Gray) ? "" : txtDateEnd.Text;
            Console.WriteLine(startDate);
            Console.WriteLine(endDate);
            if (!Equals(startDate, ""))
            {
                data.Add(startDate);
                string parameter = "@startDate";
                para.Add(parameter);
            }
            if (!Equals(endDate, ""))
            {
                data.Add(endDate);
                string parameter = "@endDate";
                para.Add(parameter);
            }
            GetMesData(data,para);
        }
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            if (dgvMesData.Rows.Count == 0)
            {
                string message = "❌ Không có dữ liệu";
                Font font = new Font("Segoe UI", 14, FontStyle.Bold);
                Color textColor = Color.FromArgb(231, 76, 60); // đỏ cam hiện đại
                SizeF textSize = e.Graphics.MeasureString(message, font);

                float x = (dgvMesData.Width - textSize.Width) / 2;
                float y = (dgvMesData.Height - textSize.Height) / 2;

                e.Graphics.DrawString(message, font, new SolidBrush(textColor), x, y);
            }
        }
        public void GetMesData(List<string> data,List<string> para)
        {
            if (data.Count >0)
            {
                //lblinform.Visible = false;
                using (SqlConnection sqlcon = new SqlConnection(mesConnect))
                {
                    try
                    {
                        sqlcon.Open();
                        string storedProcName = "RP_checkfabricinfo";
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

                            dgvMesData.DataSource = dt;
                        }
                        int rows = dgvMesData.Rows.Count;
                        int columns = dgvMesData.Columns.Count;
                        if (rows <= 0)
                        {
                            //lblinform.Visible = true;
                            dgvMesData.DataSource = null;
                            dgvMesData.Columns.Clear();
                            dgvMesData.Paint += dataGridView1_Paint;
                        }
                        else
                        {
                            if (!dgvMesData.Columns.Contains("Chon"))
                            {
                                DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
                                chkCol.HeaderText = "";
                                chkCol.Name = "Chon";
                                chkCol.Width = 60;
                                chkCol.TrueValue = true;
                                chkCol.FalseValue = false;
                                chkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                dgvMesData.Columns.Insert(0, chkCol);
                            }
        // 🔹 Nếu chưa có checkbox "chkAll" thì mới thêm
        CheckBox chkAll = dgvMesData.Controls.OfType<CheckBox>().FirstOrDefault(c => c.Name == "chkAll");
                            if (chkAll == null)
                            {
                                chkAll = new CheckBox();
                                chkAll.Name = "chkAll";
                                chkAll.Size = new Size(16, 16);
                                chkAll.BackColor = Color.Transparent;

                                // Tính toán lại vị trí checkbox trong header
                                Rectangle rect = dgvMesData.GetCellDisplayRectangle(0, -1, true);
                                // Canh giữa checkbox trong ô header
                                int centerX = rect.X + (rect.Width - chkAll.Width) / 2;
                                int centerY = rect.Y + (rect.Height - chkAll.Height) / 2;
                                chkAll.Location = new Point(centerX, centerY);

                                // Sự kiện chọn tất cả
                                chkAll.CheckedChanged += (s, e) =>
                                {
                                    dgvMesData.EndEdit(); // tránh lỗi nếu ô đang edit
                                    foreach (DataGridViewRow row in dgvMesData.Rows)
                                    {
                                        if (row.Cells["Chon"] is DataGridViewCheckBoxCell cell)
                                            cell.Value = chkAll.Checked;
                                    }
                                };

                                // Thêm checkbox vào DataGridView
                                dgvMesData.Controls.Add(chkAll);
                            }
                            else
                            {
                                // Nếu đã có rồi thì chỉ cập nhật lại vị trí khi DataGridView thay đổi
                                Rectangle rect = dgvMesData.GetCellDisplayRectangle(0, -1, true);
                                int centerX = rect.X + (rect.Width - chkAll.Width) / 2;
                                int centerY = rect.Y + (rect.Height - chkAll.Height) / 2;
                                chkAll.Location = new Point(centerX, centerY);
                            }

                            dgvMesData.Columns["Barcode"].Visible = false;
                            dgvMesData.Columns["Giờ Xả"].DefaultCellStyle.Format = "dd-MM-yyyy HH:mm"; // hoặc "dd/MM/yyyy HH:mm"
                            dgvMesData.Columns["Sẵn sàng"].DefaultCellStyle.Format = "dd-MM-yyyy HH:mm"; // hoặc "dd/MM/yyyy HH:mm"
                            dgvMesData.Columns["."].HeaderText = "STT";
                            dgvMesData.RowHeadersWidth = 25; // Đơn vị là pixel
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }else
            {
                MessageBox.Show("Vui lòng nhập dữ liệu .","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                txtMesBatch.Focus();
            }
        }
        private void CustomizeGrid()
        {
            var dgv = dgvMesData;

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
            dgv.AutoSizeColumnsMode =DataGridViewAutoSizeColumnsMode.DisplayedCells; // 🔹 nhanh hơn Fill
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; // 🔹 tránh auto height gây lag
            // Không cho người dùng chỉnh sửa hoặc thêm dòng
            dgv.ReadOnly = false;
            dgv.AllowUserToAddRows = false;
            //dgv.AllowUserToResizeRows = false;

            // Chiều cao dòng vừa đủ cho font (tránh bị cắt chữ)
            dgv.RowTemplate.Height = 28;
            // Cuộn mượt hơn
            //dgv.DoubleBuffered(true); // 🔹 custom extension bên dưới
            dgv.ResumeLayout();
        }

        private void btnMesExport_Click(object sender, EventArgs e)
        {
            if(dgvMesData.Rows.Count <=0)
            {
                MessageBox.Show("Chưa có dữ liệu. Không thể Export", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }    
            else
            {
                try
                {
                    // Thiết lập giấy phép cho EPPlus
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    //export
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveFileDialog1.Title = "Export to Excel";
                    saveFileDialog1.FileName = "MES_" + DateTime.Now.ToString("ddMMyyyy_hhmmtt") + ".xlsx";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = saveFileDialog1.FileName;

                        using (var package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MES");
                            int cellRowIndex = 5;
                            int cellColumnIndex = 1;

                            // Xuất tiêu đề cột
                            for (int i = 1; i < dgvMesData.Columns.Count; i++)
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex].Value = dgvMesData.Columns[i].HeaderText;
                                cellColumnIndex++;
                            }

                            // Xuất dữ liệu từ GridView vào Excel
                            for (int i = 0; i < dgvMesData.Rows.Count; i++)
                            {
                                cellColumnIndex = 1;
                                cellRowIndex++;

                                for (int j = 1; j < dgvMesData.Columns.Count; j++)
                                {
                                    if (dgvMesData.Rows[i].Cells[j].Value != null)
                                    {
                                        worksheet.Cells[cellRowIndex, cellColumnIndex].Value = dgvMesData.Rows[i].Cells[j].Value.ToString();
                                    }
                                    cellColumnIndex++;
                                }
                            }
                            //var workSheet = excel.Workbook.Worksheets.Add("Washing");
                            var totalCols = dgvMesData.Columns.Count;
                            var totalRows = dgvMesData.Rows.Count;
                            //var workSheet = excel.Workbook.Worksheets.Add("CuttingReport");
                            worksheet.Cells[1, 1, 1, dgvMesData.Columns.Count].Merge = true;
                            worksheet.Cells[1, 1].Value = "REGENT GARMENT FACTOTY LTD";
                            worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            //workSheet.Cells["B2:" + characters + "2"].Merge = true;
                            worksheet.Cells[2, 1, 2, dgvMesData.Columns.Count].Merge = true;
                            worksheet.Cells[2, 1].Value = "(A SUBSIDIARY OF CRYSTAL INTERNATIONAL GROUP LTD)";
                            worksheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            //workSheet.Cells["B3:" + characters + "3"].Merge = true;
                            worksheet.Cells[3, 2, 3, dgvMesData.Columns.Count - 1].Merge = true;
                            worksheet.Cells[3, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            if (totalCols < 6)
                            {
                                worksheet.Cells[1, 1].Style.Font.Size = 12;
                                worksheet.Cells[2, 1].Style.Font.Size = 11;
                                worksheet.Cells[3, 2].Style.Font.Size = 11;
                            }
                            else
                            {
                                worksheet.Cells[1, 1].Style.Font.Size = 16;
                                worksheet.Cells[2, 1].Style.Font.Size = 12;
                                worksheet.Cells[3, 2].Style.Font.Size = 12;
                            }
                            worksheet.Cells[1, 1].Style.Font.Bold = true;
                            worksheet.Cells[2, 1].Style.Font.Bold = true;
                            worksheet.Cells[3, 2].Style.Font.Bold = true;
                            worksheet.Cells[3, 1].Value = "Date: " + DateTime.Now.ToString("dd-MM-yyyy");
                            worksheet.Cells[4, 1].Value = "Time: " + DateTime.Now.ToString("hh:mm:ss tt");
                            //worksheet.Cells[3, dataGridView1.Columns.Count].Value = "Fr: " + CalendarFrom.SelectedDate.ToString("dd-MM-yyyy");
                            //worksheet.Cells[4, dataGridView1.Columns.Count].Value = "To: " + CalendarTo.SelectedDate.ToString("dd-MM-yyyy");
                            worksheet.Cells[3, 1].Style.Font.Italic = true;
                            worksheet.Cells[4, 1].Style.Font.Italic = true;
                            worksheet.Cells[3, dgvMesData.Columns.Count].Style.Font.Italic = true;
                            worksheet.Cells[4, dgvMesData.Columns.Count].Style.Font.Italic = true;
                            for (var col = 1; col <= totalCols - 1; col++)
                            {
                                //worksheet.Cells[5, col].Value = dataGridView1.Columns[col - 1].ColumnName;
                                worksheet.Cells[5, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[5, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                                worksheet.Cells[5, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[5, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[5, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[5, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[5, col].Style.Font.Bold = true;
                            }
                            worksheet.Cells[3, 2].Value = "Fabric Information Report";
                            // Thêm auto filter cho tiêu đề
                            //int headerRow = 5;
                            //int endColumn = dataGridView1.Columns.Count;

                            //using (ExcelRange range = worksheet.Cells[headerRow, 1, headerRow, endColumn])
                            //{
                            //    range.AutoFilter = true;
                            //}
                            worksheet.Cells[5, 1, 5, dgvMesData.Columns.Count - 1].AutoFilter = true;
                            worksheet.View.FreezePanes(6, 1);
                            worksheet.Cells.AutoFitColumns();
                            worksheet.Protection.IsProtected = false;
                            worksheet.Protection.AllowSelectLockedCells = false;
                            //package.Save();
                            // Lưu tệp Excel bằng FileStream
                            using (var fileStream = new FileStream(fileName, FileMode.Create))
                            {
                                package.SaveAs(fileStream);
                            }
                        }

                        // Mở tệp Excel sau khi lưu
                        Process.Start(fileName);
                        //Process.Start(saveFileDialog1.FileName);
                        //MessageBox.Show("Dữ liệu đã được xuất thành công vào tệp Excel!");
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi kỳ lạ nào đó đã xảy ra: " + ex.Message);
                }
            }
        }

        private void btnMesPrint_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvMesData.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Chon"].Value ?? false);

                if (isChecked)
                {
                    string name = Convert.ToString(row.Cells["Barcode"].Value);
                    MessageBox.Show($"Đã chọn: {name}");
                }
            }
        }
    }
    // 🔹 Extension giúp bật DoubleBuffered an toàn cho mọi Control
    public static class ControlExtensions
    {
        public static void SetDoubleBuffered(this Control control, bool setting)
        {
            var prop = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(control, setting, null);
        }
    }
}

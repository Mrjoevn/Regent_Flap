using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FLap_New.Object;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

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
            SetupWatermark(txtDateStart);
            SetupWatermark(txtDateEnd);
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
                targetTextBox.Text = e.Start.ToString("dd-MM-yyyy");
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
        private string watermarkText = "dd-mm-yyyy";
        private void SetupWatermark(TextBox textBox)
        {
            // Nếu ban đầu trống thì gán watermark
            if (string.IsNullOrWhiteSpace(textBox.Text))
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
                if (string.IsNullOrWhiteSpace(textBox.Text))
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
            //// 🔑 Ma trận khóa 4x4 (phải khả nghịch mod 26)
            ////Console.Write("Nhập chuỗi cần mã hóa (A–Z): ");
            //string plaintext = "study123";
            //HillCipher4x4 hill_Encryption = new HillCipher4x4();
            //string ciphertext = hill_Encryption.Encrypt(plaintext.ToUpper());
            //txtMesSo.Text = "Ciphertext: " + ciphertext;
            //string decrypted = hill_Encryption.Decrypt(ciphertext);
            //txtMesBatch.Text = "Decrypted: " + decrypted;
            ConnectionToSql conn = new ConnectionToSql("CicsConnectionString");
            Console.WriteLine(conn.GetRawConnectionString());
        }
        //public DataTable GetData()
        //{
        //    using (SqlConnection sqlcon = new SqlConnection(connection))
        //    {
        //        sqlcon.Open();
        //        SqlCommand cmd = new SqlCommand(" set nocount on exec RP_checkfabricinfo '" + txtdate.Text + "','" + sotxt.Text + "','" + batchtxt.Text + "','" + trolleytxt.Text + "','" + txtroll.Text + "'", sqlcon);
        //        SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        DataTable dttb = new DataTable();
        //        sqlda.Fill(dttb);
        //        dtviewMES.DataSource = dttb;
        //        return dttb;
        //    }
        //}
    }
}

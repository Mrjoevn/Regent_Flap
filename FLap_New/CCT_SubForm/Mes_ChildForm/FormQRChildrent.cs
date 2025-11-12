using Google.Protobuf.WellKnownTypes;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLap_New.SubForm.Mes_ChildForm
{
    public partial class FormQRChildrent : Form
    {
        public FormQRChildrent()
        {
            InitializeComponent();
            this.Font = ConstantsFormQRChild.FONT_DEFAULT; // Đặt font toàn cục cho form
            ResetAllMarginsAndPadding(this);  // Đặt lại toàn bộ Margin và Padding

            int panelWidth = ConstantsFormQRChild.PANEL_WIDTH; ///Kich thuoc ma QR
            int panelHeight = ConstantsFormQRChild.PANEL_HEGHT;

            // Panel ở góc trên trái
            pnTopLeft.Size = new Size(panelWidth, panelHeight);
            pnTopLeft.Location = new Point(0, 0);  // Góc trên trái
            pnTopLeft.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pnTopLeft.BringToFront();

            //// Panel ở góc trên phải
            //pnTopRight.Size = new Size(panelWidth, panelHeight);
            //pnTopRight.Location = new Point(this.ClientSize.Width - panelWidth, 0);  // Góc trên phải
            //pnTopRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            //pnTopRight.BringToFront();

            //// Panel ở góc dưới trái
            //pnBottomLeft.Size = new Size(panelWidth, panelHeight);
            //pnBottomLeft.Location = new Point(0, this.ClientSize.Height - panelHeight);  // Góc dưới trái
            //pnBottomLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //pnBottomLeft.BringToFront();

            //// Panel ở góc dưới phải
            //pnBottomRight.Size = new Size(panelWidth, panelHeight);
            //pnBottomRight.Location = new Point(this.ClientSize.Width - panelWidth, this.ClientSize.Height - panelHeight);  // Góc dưới phải
            //pnBottomRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //pnBottomRight.BringToFront();

            ////config QR text Barcode+Roll_NO
            //this.lbQRText.Font = ConstantsFormQRChild.FONT_QRCODE_TEXT;
            //lbQRText.Anchor = AnchorStyles.None; // Không cố định vào bất kỳ cạnh nào
            //lbQRText.TextAlign = ContentAlignment.MiddleCenter; // Căn giữa nội dung
            //lbQRText.Dock = DockStyle.Fill;
        }

        public void LoadData(FabricInfoDTO fabricInfoDTO)
        {
            //handle get data value
            //this.lbQRText.Text = fabricInfoDTO.Batch.ToUpper() + " - " + fabricInfoDTO.Roll.PadLeft(3, '0');
            this.lbQRText.Text = fabricInfoDTO.Barcode.ToUpper();

            //this.lbVendorValue.Text = fabricInfoDTO.Vendor.ToUpper();
            this.lbSoValue.Text = fabricInfoDTO.So.ToUpper();
            this.lbRollValue.Text = fabricInfoDTO.Roll;
            //this.lbCustomerValue.Text = fabricInfoDTO.Customer.ToUpper();
            //this.lbGroupValue.Text = fabricInfoDTO.Job_Group.ToUpper();
            this.lbBatchValue.Text = fabricInfoDTO.Batch.ToUpper();
            this.lbLengthValue.Text = fabricInfoDTO.Length.ToString("F4");
            //this.lbWeightValue.Text = fabricInfoDTO.Weight.ToString("F4");
            this.lbColorValue.Text = fabricInfoDTO.Color.ToUpper();
            this.lbStyleValue.Text = fabricInfoDTO.Style.ToUpper();

            //handle get QR Code
            Image qrImage = GenerateQRCode(fabricInfoDTO);

            AddPictureBoxToPanel(this.pnTopLeft, qrImage);
            //AddPictureBoxToPanel(this.pnTopRight, qrImage);
            //AddPictureBoxToPanel(this.pnBottomLeft, qrImage);
            //AddPictureBoxToPanel(this.pnBottomRight, qrImage);
        }

        private void AddPictureBoxToPanel(Panel panel, Image qrImage)
        {
            PictureBox pictureBox = new PictureBox
            {
                Image = qrImage,                             // Gán hình ảnh QR code cho PictureBox
                SizeMode = PictureBoxSizeMode.StretchImage,  // Ảnh sẽ lấp đầy PictureBox
                Dock = DockStyle.Fill                        // PictureBox sẽ lấp đầy toàn bộ Panel
            };

            panel.Controls.Add(pictureBox);
        }

        private void ResetAllMarginsAndPadding(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Margin = new Padding(0);   // Đặt khoảng trống bên ngoài = 0
                control.Padding = new Padding(0);  // Đặt khoảng trống bên trong = 0

                // Nếu control này chứa các control con, gọi đệ quy
                if (control.Controls.Count > 0)
                {
                    ResetAllMarginsAndPadding(control);
                }
            }
        }

        private Bitmap GenerateQRCode(FabricInfoDTO info)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                //QRCodeData qrCodeData = qrGenerator.CreateQrCode(info.Batch.ToUpper() + " - " + info.Roll.PadLeft(3, '0'),
                //    QRCodeGenerator.ECCLevel.Q);

                QRCodeData qrCodeData = qrGenerator.CreateQrCode(info.Barcode.ToUpper(), QRCodeGenerator.ECCLevel.Q);

                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Tạo một hình ảnh từ mã QR
                    return qrCode.GetGraphic(100);
                }

            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace FLap_New.SubForm.Mes_ChildForm
{
    public partial class FormQR : Form
    {
        private Panel panel;
        int formWidth = ConstantsFormQRChild.WIDTH;   // Chiều rộng cố định của form con
        int formHeight = ConstantsFormQRChild.HEGHT;  // Chiều cao cố định của form con
        int spacing = ConstantsFormQRChild.SPACING;   // Khoảng cách giữa các form con

        public FormQR()
        {
            InitializeComponent();

            panel = new Panel();
            panel.Dock = DockStyle.Fill; // Panel chiếm toàn bộ form cha
            panel.AutoScroll = true;     // Cho phép cuộn

            this.Controls.Add(panel);    // Thêm panel vào form cha

            // Gắn sự kiện Resize để sắp xếp lại form con
            this.Resize += FormQR_Resize;
        }

        public void LoadData(List<FabricInfoDTO> listPictureBox)
        {
            // Lấy kích thước chiều rộng của panel
            int parentWidth = panel.ClientSize.Width;

            // Tọa độ ban đầu cho form con
            int xPosition = spacing;
            int yPosition = spacing;

            for (var i = 0; i < listPictureBox.Count(); i++)
            {
                FormQRChildrent form = new FormQRChildrent();
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                form.Size = new Size(formWidth, formHeight);

                // Nếu không đủ không gian chiều ngang, xuống dòng (tăng y và đặt lại x)
                if (xPosition + formWidth + spacing > parentWidth)
                {
                    xPosition = spacing; // Đặt lại vị trí x về đầu dòng mới
                    yPosition += formHeight + spacing; // Xuống dòng mới, tăng y theo chiều cao form con
                }

                // Đặt vị trí cho form con
                form.Location = new Point(xPosition, yPosition);

                // Load dữ liệu cho form con
                form.LoadData(listPictureBox[i]);

                // Thêm form con vào panel
                panel.Controls.Add(form);
                form.Show();

                // Cập nhật xPosition cho form con tiếp theo
                xPosition += formWidth + spacing;
            }
        }

        private void FormQR_Resize(object sender, EventArgs e)
        {
            RearrangeChildForms();
        }

        // Hàm sắp xếp các form con trong panel
        private void RearrangeChildForms()
        {
            int parentWidth = panel.ClientSize.Width; // Chiều rộng của panel

            int xPosition = spacing;
            int yPosition = spacing;

            // Kiểm tra từng điều khiển trong panel
            foreach (Control control in panel.Controls)
            {
                // Kiểm tra xem có phải form con (FormQRChildrent) không
                if (control is FormQRChildrent childForm)
                {
                    // Nếu không đủ chỗ ở chiều ngang, chuyển xuống dòng
                    if (xPosition + formWidth + spacing > parentWidth)
                    {
                        xPosition = spacing; // Đặt lại x
                        yPosition += formHeight + spacing; // Xuống dòng
                    }

                    // Đặt vị trí cho form con
                    childForm.Location = new Point(xPosition, yPosition);

                    // Cập nhật vị trí x cho form con tiếp theo
                    xPosition += formWidth + spacing;
                }
            }
        }
        public class RawPrinterHelper
        {
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
            public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter")]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] ref DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter")]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter")]
            public static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter")]
            public static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter")]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

            [StructLayout(LayoutKind.Sequential)]
            public struct DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;

                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;

                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }

            public static bool SendStringToPrinter(string printerName, string zplCommand)
            {
                IntPtr pBytes;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                int dwWritten = 0;

                di.pDocName = "ZPL Label";
                di.pDataType = "RAW";

                bool success = false;

                if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    if (StartDocPrinter(hPrinter, 1, ref di))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(zplCommand);
                            pBytes = Marshal.AllocCoTaskMem(bytes.Length);
                            Marshal.Copy(bytes, 0, pBytes, bytes.Length);

                            success = WritePrinter(hPrinter, pBytes, bytes.Length, out dwWritten);
                            EndPagePrinter(hPrinter);
                            Marshal.FreeCoTaskMem(pBytes);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }

                return success;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string printerName = "HP LaserJet Pro M501 PCL 6"; // Tên máy in trong Devices and Printers

            string zpl = "^XA" +
                         "^ FO50,20 ^ A0N,30,30 ^ FD D50419688 - 035 ^ FS" +
                        "^ FO50,60" +
                        "^ BQN,2,5" +
                        "^ FDQA,D50419688 - 035 ^ FS" +
                        "^ FO200,60 ^ A0N,25,25 ^ FDSO No: 5V2505521 ^ FS" +
                        "^ FO200,90 ^ A0N,25,25 ^ FDBatch No: D50419688 ^ FS" +
                        "^ FO200,120 ^ A0N,25,25 ^ FDRoll No: 35 ^ FS" +
                        "^ FO200,150 ^ A0N,25,25 ^ FDLength     : 90,6300 YD ^ FS" +
                        "^ FO200,180 ^ A0N,25,25 ^ FDStyle      : 04345N149F ^ FS" +
                        "^ FO200,210 ^ A0N,25,25 ^ FDColor      : 08 DARK GRAY^FS" +
                        "^ XZ";

            RawPrinterHelper.SendStringToPrinter(printerName, zpl);

            string ipAddress = "192.168.150.46"; // IP của máy in Zebra
            int port = 9100;

            TcpClient client = new TcpClient();
            client.Connect(ipAddress, port);
            StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.ASCII);
            writer.Write(zpl);
            writer.Flush();

            writer.Close();
            client.Close();
        }
    }
}
